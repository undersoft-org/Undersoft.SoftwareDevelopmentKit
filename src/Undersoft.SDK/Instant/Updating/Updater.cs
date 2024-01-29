using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Instant.Updating;

using Invoking;
using Proxies;
using Instant.Rubrics;

public class Updater : IUpdater
{
    protected HashSet<int> trackset;
    protected ProxyCreator creator;
    protected IProxy source;
    protected Type type => creator.BaseType;
    protected int counter = 0;
    protected bool traceable;

    public IProxy Source
    {
        get => source;
        set => source = value;
    }

    public Action<Updater, object> UpdateAction { get; set; }

    public IInvoker TraceEvent { get; set; }

    public Updater() { }

    public Updater(object item, IInvoker traceChanges) : this(item.GetType())
    {
        if (traceChanges != null)
        {
            TraceEvent = traceChanges;
            traceable = true;
        }
        if (item.GetType().IsAssignableTo(typeof(IProxy)))
            Combine(item as IProxy);
        else
            Combine(item);
    }

    public Updater(object item) : this(item, null)
    {
    }

    public Updater(IProxy sleeve) : this(sleeve.Target.GetType())
    {
        Combine(sleeve);
    }

    public Updater(Type type)
    {
        creator = ProxyFactory.GetCreator(type);
    }

    public void Combine(IProxy sleeve)
    {
        source = sleeve;
    }

    public void Combine(object item)
    {
        source = creator.Create(item);
    }

    protected void setBy(IProxy target, UpdaterItem[] changes, int count)
    {
        var _target = target;
        var _changes = changes;
        UpdaterItem vary;
        for (int i = 0; i < count; i++)
        {
            vary = _changes[i];
            if (vary.TargetType.IsAssignableTo(vary.OriginType))
            {
                _target[vary.TargetIndex] = vary.OriginValue;
            }
        }
    }

    protected void set(IProxy target, UpdaterItem[] changes, int count)
    {
        var _target = target;
        var _changes = changes;
        UpdaterItem vary;
        for (int i = 0; i < count; i++)
        {
            vary = _changes[i];
            _target[vary.TargetIndex] = vary.OriginValue;
        }
    }

    public object Patch(object item)
    {
        UpdaterItem[] changes;

        UpdateAction = (o, t) => o.Patch(t);

        IProxy target = item.ToProxy();
        if (item.GetType() != type)
            setBy(target, changes = PatchNotEqualTypes(target), counter);
        else
            set(target, changes = PatchEqualTypes(target), counter);

        return item;
    }

    public E Patch<E>(E item) where E : class
    {
        UpdaterItem[] changes;

        UpdateAction = (o, t) => o.Patch(t);

        IProxy target = item.ToProxy();
        if (typeof(E) != type)
            setBy(target, changes = PatchNotEqualTypes(target), counter);
        else
            set(target, changes = PatchEqualTypes(target), counter);

        return item;
    }

    public E Patch<E>() where E : class
    {
        return Patch(typeof(E).New<E>());
    }

    public object PatchSelf()
    {
        UpdateAction = (o, s) => o.PatchSelf();

        set(source, PatchEqualTypes(source), counter);
        return Target;
    }

    public object Put(object item)
    {
        UpdaterItem[] changes = null;

        UpdateAction = (o, t) => o.Put(t);

        IProxy target = item.ToProxy();
        if (target != null)
        {
            if (item.GetType() != type)
                setBy(target, changes = PutEqualTypes(target), counter);
            else
                set(target, changes = PutNotEqualTypes(target), counter);
        }
        return item;
    }

    public E Put<E>(E item) where E : class
    {
        UpdaterItem[] changes = null;

        UpdateAction = (o, t) => o.Put(t);

        IProxy target = item.ToProxy();
        if (target != null)
        {
            if (typeof(E) != type)
                setBy(target, changes = PutEqualTypes(target), counter);
            else
                set(target, changes = PutNotEqualTypes(target), counter);
        }
        return item;
    }

    public E Put<E>() where E : class
    {
        return Put(typeof(E).New<E>());
    }

    public object PutSelf()
    {
        UpdateAction = (c, h) => c.PutSelf();

        set(source, PutNotEqualTypes(source), counter);
        return Target;
    }

    public UpdaterItem[] Detect(object item)
    {
        UpdaterItem[] changes = null;

        UpdateAction = (o, t) => o.Detect(t);

        IProxy target = item.ToProxy();
        if (target != null)
        {
            if (item.GetType() != type)
                changes = PatchNotEqualTypes(target);
            else
                changes = PatchEqualTypes(target);
        }
        return changes;
    }

    public UpdaterItem[] Detect<E>(E item) where E : class
    {
        UpdaterItem[] changes = null;

        UpdateAction = (o, t) => o.Detect(t);

        IProxy target = item.ToProxy();
        if (target != null)
        {
            if (typeof(E) != type)
                changes = PatchNotEqualTypes(target);
            else
                changes = PatchEqualTypes(target);
        }
        return changes;
    }

    public object Clone()
    {
        var clone = type.New();
        var _clone = creator.Create(clone);
        _clone.PutFrom(source);
        return clone;
    }

    protected UpdaterItem[] PatchEqualTypes(IProxy target)
    {
        counter = 0;
        var _target = target;
        var _sameVariations = new UpdaterItem[Rubrics.Count];
        var _item = new UpdaterItem();

        Rubrics.ForEach(
            (rubric) =>
            {
                if (!rubric.IsKey && !ExcludedRubrics.Contains(rubric.Name.ToLower()))
                {
                    var targetndex = rubric.RubricId;
                    var originValue = Source[targetndex];
                    var targetValue = _target[targetndex];

                    if (
                        !originValue.NullOrEquals(targetValue)
                    )
                    {
                        if (!RecursiveUpdate(originValue, targetValue, target, rubric, rubric))
                        {
                            _item.TargetIndex = targetndex;
                            _item.OriginValue = originValue;
                            _sameVariations[counter++] = _item;
                        }
                    }
                }
            }
        );
        return _sameVariations;
    }

    protected UpdaterItem[] PatchNotEqualTypes(IProxy target)
    {
        counter = 0;
        var _target = target;
        var _customVariations = new UpdaterItem[Rubrics.Count];
        var _item = new UpdaterItem();

        Rubrics.ForEach(
            (originRubric) =>
            {
                if (!originRubric.IsKey && !ExcludedRubrics.Contains(originRubric.Name.ToLower()))
                {
                    var name = originRubric.Name;
                    if (_target.Rubrics.TryGet(name, out MemberRubric targetRubric))
                    {
                        var originValue = Source[originRubric.RubricId];
                        var targetIndex = targetRubric.RubricId;
                        var targetValue = _target[targetIndex];

                        if (
                            !originValue.NullOrEquals(targetValue)                            
                        )
                        {
                            if (
                                !RecursiveUpdate(
                                    originValue,
                                    targetValue,
                                    target,
                                    originRubric,
                                    targetRubric
                                )
                            )
                            {
                                _item.TargetIndex = targetIndex;
                                _item.OriginValue = originValue;
                                _item.OriginType = originRubric.RubricType;
                                _item.TargetType = targetRubric.RubricType;
                                _customVariations[counter++] = _item;
                            }
                        }
                    }
                }
            }
        );
        return _customVariations;
    }

    protected UpdaterItem[] PutNotEqualTypes(IProxy target)
    {
        counter = 0;
        var _target = target;
        var _sameMutations = new UpdaterItem[Rubrics.Count];
        var _item = new UpdaterItem();

        Rubrics.ForEach(
            (rubric) =>
            {
                if (!rubric.IsKey && !ExcludedRubrics.Contains(rubric.Name.ToLower()))
                {
                    var index = rubric.RubricId;
                    var originValue = Source[index];
                    var targetValue = _target[index];

                    if (
                        originValue != null
                        && !RecursiveUpdate(originValue, targetValue, target, rubric, rubric)
                    )
                    {
                        _item.TargetIndex = index;
                        _item.OriginValue = originValue;
                        _sameMutations[counter++] = _item;
                    }
                }
            }
        );
        return _sameMutations;
    }

    protected UpdaterItem[] PutEqualTypes(IProxy target)
    {
        counter = 0;
        var _target = target;
        var _customMutations = new UpdaterItem[Rubrics.Count];
        var _item = new UpdaterItem();

        Rubrics.ForEach(
            (originRubric) =>
            {
                if (!originRubric.IsKey && !ExcludedRubrics.Contains(originRubric.Name.ToLower()))
                {
                    var name = originRubric.Name;
                    if (_target.Rubrics.TryGet(name, out MemberRubric targetRubric))
                    {
                        var originValue = Source[originRubric.RubricId];
                        var targetIndex = targetRubric.RubricId;
                        var targetValue = _target[targetIndex];

                        if (
                            originValue != null
                            && !RecursiveUpdate(
                                originValue,
                                targetValue,
                                target,
                                originRubric,
                                targetRubric
                            )
                        )
                        {
                            _item.TargetIndex = targetIndex;
                            _item.OriginValue = originValue;
                            _item.OriginType = originRubric.RubricType;
                            _item.TargetType = targetRubric.RubricType;
                            _customMutations[counter++] = _item;
                        }
                    }
                }
            }
        );
        return _customMutations;
    }

    private bool RecursiveUpdate(
        object originValue,
        object targetValue,
        IProxy target,
        IRubric originRubric,
        IRubric targetRubric
    )
    {
        var originType = originRubric.RubricType;
        var targetType = targetRubric.RubricType;

        if (originType.IsValueType || (originType == typeof(string)))
            return false;

        if (targetValue == null)
        {
            if (traceable)
                targetValue = TraceEvent.Invoke(target.Target, targetRubric.RubricName, targetType);

            if (targetValue == null)
                target[targetRubric.RubricId] = targetValue = targetType.New();
        }

        if (originType.IsAssignableTo(typeof(ICollection)))
        {
            ICollection originItems = (ICollection)originValue;
            var originItemType = originType.GetEnumerableElementType();
            if (originItemType == null || !originItemType.IsValueType)
            {
                if (targetType.IsAssignableTo(typeof(ICollection)))
                {
                    ICollection targetItems = (ICollection)targetValue;
                    var targetItemType = targetType.GetEnumerableElementType();
                    if (targetItemType == null || !targetItemType.IsValueType)
                    {
                        if (
                            targetType.IsAssignableTo(typeof(IFindableSeries))
                            && originItemType.IsAssignableTo(typeof(IIdentifiable))
                        )
                        {
                            IFindableSeries targetFindable = (IFindableSeries)targetValue;

                            foreach (var originItem in originItems)
                            {
                                var targetItem = targetFindable[originItem];
                                if (targetItem != null)
                                {
                                    if (traceable)
                                        targetItem = TraceEvent.Invoke(targetItem, null, null);

                                    UpdateAction(
                                        new Updater(originItem, TraceEvent),
                                        targetItem
                                    );
                                }
                                else if (originItemType != targetItemType)
                                {
                                    targetItem = targetItemType.New();

                                    ((IIdentifiable)targetItem).Id = ((IIdentifiable)originItem).Id;

                                    originItem.PatchTo(targetItem, TraceEvent);

                                    if (traceable)
                                        targetItem = TraceEvent.Invoke(targetItem, null, null);

                                    ((IList)targetItems).Add(targetItem);
                                }
                                else
                                {
                                    if (traceable)
                                        targetItem = TraceEvent.Invoke(originItem, null, null);

                                    ((IList)targetItems).Add(originItem);
                                }
                            }

                            return true;
                        }

                        GreedyLookup(originItems, targetItems, originItemType, targetItemType);
                    }
                }
            }
        }

        if (traceable)
            targetValue = TraceEvent.Invoke(targetValue, null, null);

        UpdateAction(new Updater(originValue, TraceEvent), targetValue);

        return false;
    }

    private bool GreedyLookup(
        ICollection originItems,
        ICollection targetItems,
        Type originItemType,
        Type targetItemType
    )
    {
        if (
            !originItemType.IsAssignableTo(typeof(IIdentifiable))
            || !targetItemType.IsAssignableTo(typeof(IIdentifiable))
        )
            return false;

        foreach (var originItem in originItems)
        {
            bool founded = false;
            foreach (var targetItem in targetItems)
            {
                if (((IIdentifiable)originItem).Id == ((IIdentifiable)targetItem).Id)
                {
                    var _targetItem = targetItem;
                    if (traceable)
                        _targetItem = TraceEvent.Invoke(_targetItem, null, null);

                    UpdateAction(new Updater(originItem, TraceEvent), _targetItem);

                    founded = true;
                    break;
                }
            }

            if (!founded)
            {
                object targetItem = null;
                if (originItemType != targetItemType)
                {
                    targetItem = targetItemType.New();
                    ((IUnique)targetItem).Id = ((IUnique)originItem).Id;
                    if (traceable)
                        targetItem = TraceEvent.Invoke(targetItem, null, null);
                    ((IList)targetItems).Add(originItem.PatchTo(targetItem, TraceEvent));
                }
                else
                {
                    if (traceable)
                        targetItem = TraceEvent.Invoke(originItem, null, null);
                    ((IList)targetItems).Add(originItem);
                }
            }
        }

        return true;
    }

    private static HashSet<string> excludedRubrics;

    public HashSet<string> ExcludedRubrics
    {
        get =>
            excludedRubrics ??= new HashSet<string>(
                new string[]
                {       
                    "proxy",
                "valuearray" 
                }                
            );
    }

    public bool Equals(IUnique other)
    {
        return source.Equals(other);
    }

    public int CompareTo(IUnique? other)
    {
        return source.CompareTo(other);
    }

    public long Id
    {
        get => source.Id;
        set => source.Id = value;
    }

    public long TypeId
    {
        get => source.TypeId;
        set => source.TypeId = value;
    }

    public object this[string propertyName]
    {
        get => source[propertyName];
        set => source[propertyName] = value;
    }

    public object this[int fieldId]
    {
        get => source[fieldId];
        set => source[fieldId] = value;
    }

    public Uscn Code
    {
        get => source.Code;
        set => source.Code = value;
    }

    public IRubrics Rubrics
    {
        get => source.Rubrics;
        set => source.Rubrics = value;
    }

    public object Target
    {
        get => source.Target;
        set => source.Target = value;
    }
}
