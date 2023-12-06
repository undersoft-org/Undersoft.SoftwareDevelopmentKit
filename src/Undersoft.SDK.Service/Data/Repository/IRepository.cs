using Microsoft.EntityFrameworkCore.ChangeTracking;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Repository.Pagination;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Repository
{
    public interface IRepository<TEntity> : IPage<TEntity>, IRepository, IOrderedQueryable<TEntity>, IEnumerable<TEntity> where TEntity : class, IOrigin
    {
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector] { get; }
        TEntity this[params object[] keys] { get; set; }
        IQueryable<TEntity> this[SortExpression<TEntity> sortTerms] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector] { get; }
        TEntity this[bool reverse, SortExpression<TEntity> sortTerms] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, object>> selector, IEnumerable<object> values] { get; }
        IGrouping<dynamic, TEntity> this[Func<IQueryable<TEntity>, IGrouping<dynamic, TEntity>> groupByObject, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<TEntity> this[IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<object> this[IQueryable<TEntity> query, Expression<Func<TEntity, int, object>> selector] { get; }
        IQueryable<object> this[IQueryable<TEntity> query, Expression<Func<TEntity, object>> selector] { get; }
        IQueryable<TEntity> this[IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[object[] keys, Expression<Func<TEntity, object>>[] expanders] { get; set; }
        IQueryable<TEntity> this[SortExpression<TEntity> sortTerms, Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, object>> selector, IEnumerable<object> values, params Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[Expression<Func<TEntity, object>> selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders] { get; set; }
        ISeries<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate] { get; }
        ISeries<TEntity> this[int skip, int take, Expression<Func<TEntity, object>>[] expanders] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector] { get; }
        IQueryable<TEntity> this[int skip, int take, IQueryable<TEntity> query] { get; }
        ISeries<TEntity> this[int skip, int take, SortExpression<TEntity> sortTerms] { get; }
        IQueryable<TEntity> this[IQueryable<TEntity> query, Expression<Func<TEntity, object>> selector, IEnumerable<object> values] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        ISeries<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        ISeries<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        ISeries<TEntity> this[int skip, int take, SortExpression<TEntity> sortTerms, Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        ISeries<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        
        IQueryable<TEntity> Query { get; }

        IEnumerable<TEntity> Add(IEnumerable<TEntity> entity);
        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        TEntity Add(TEntity entity);
        TEntity Add(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IAsyncEnumerable<TEntity> AddAsync(IAsyncEnumerable<TEntity> entity);
        IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entity);
        IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<IEnumerable<TEntity>> AddBy<TDto>(IEnumerable<TDto> model);
        Task<IEnumerable<TEntity>> AddBy<TDto>(IEnumerable<TDto> models, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<TEntity> AddBy<TDto>(TDto model);
        Task<TEntity> AddBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IAsyncEnumerable<TEntity> AddByAsync<TDto>(IEnumerable<TDto> model);
        IAsyncEnumerable<TEntity> AddByAsync<TDto>(IEnumerable<TDto> models, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        IQueryable<TEntity> AsQueryable();
        TEntity Delete(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entity);
        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IEnumerable<TEntity> Delete(long[] ids);
        Task<TEntity> Delete(params object[] key);
        TEntity Delete(TEntity entity);
        TEntity Delete(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IAsyncEnumerable<TEntity> DeleteAsync(IEnumerable<TEntity> entity);
        IAsyncEnumerable<TEntity> DeleteAsync(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IEnumerable<TEntity> DeleteBy<TDto>(IEnumerable<TDto> model);
        IEnumerable<TEntity> DeleteBy<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<TEntity> DeleteBy<TDto>(TDto model);
        Task<TEntity> DeleteBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IAsyncEnumerable<TEntity> DeleteByAsync<TDto>(IEnumerable<TDto> model);
        IAsyncEnumerable<TEntity> DeleteByAsync<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<bool> Exist(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Exist(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<bool> Exist(Type exceptionType, Expression<Func<TEntity, bool>> predicate, string message);
        Task<bool> Exist(Type exceptionType, object instance, string message);
        Task<bool> Exist<TException>(Expression<Func<TEntity, bool>> predicate, string message) where TException : Exception;
        Task<bool> Exist<TException>(object instance, string message) where TException : Exception;
        Task<ISeries<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate);
        Task<ISeries<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms);
        Task<ISeries<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Filter(int skip, int take, SortExpression<TEntity> sortTerms);
        Task<ISeries<TEntity>> Filter(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Filter(IQueryable<TEntity> query);
        Task<IList<TDto>> Filter<TDto, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IList<TDto>> Filter<TDto, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IList<TDto>> Filter<TDto, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IList<TDto>> Filter<TDto, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IList<TDto>> Filter<TDto, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, SortExpression<TEntity> sortTerms, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, Expression<Func<TEntity, bool>> predicate);
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms);
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, SortExpression<TEntity> sortTerms);
        Task<ISeries<TDto>> Filter<TDto>(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TEntity>> Filter<TDto>(IQueryable<TDto> query);
        Task<IList<TDto>> Filter<TDto>(IQueryable<TEntity> query);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        IAsyncEnumerable<TDto> FilterAsync<TDto>(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        IAsyncEnumerable<TDto> FilterAsync<TDto>(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool reverse = false);
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool reverse = false, params Expression<Func<TEntity, object>>[] expanders);
        Task<TEntity> Find(object[] keys, params Expression<Func<TEntity, object>>[] expanders);
        Task<TEntity> Find(params object[] keys);
        Task<TDto> Find<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<TDto> Find<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<TDto> Find<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<TDto> Find<TDto>(Expression<Func<TEntity, bool>> predicate, bool reverse);
        Task<TDto> Find<TDto>(Expression<Func<TEntity, bool>> predicate, bool reverse, params Expression<Func<TEntity, object>>[] expanders);
        Task<TDto> Find<TDto>(object[] keys, params Expression<Func<TEntity, object>>[] expanders);
        Task<TDto> Find<TDto>(params object[] keys);
        Task<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        Task<TResult> Find<TResult>(object[] keys, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        IQueryable<TDto> FindOneAsync<TDto>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TDto : class, IOrigin;
        IQueryable<TDto> FindOneAsync<TDto>(object[] keys, params Expression<Func<TEntity, object>>[] expanders) where TDto : class, IOrigin;
        Task<ISeries<TEntity>> Get(int skip, int take, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Get(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Get(params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TEntity>> Get(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TDto>> Get<TDto, TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IList<TDto>> Get<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<ISeries<TDto>> Get<TDto>(int skip, int take, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TDto>> Get<TDto>(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TResult>> Get<TResult>(Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Get<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        IAsyncEnumerable<TDto> GetAsync<TDto>(int skip, int take, params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        IAsyncEnumerable<TDto> GetAsync<TDto>(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        IQueryable<TDto> GetQuery<TDto>(params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        IQueryable<TDto> GetQuery<TDto>(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        Task<IQueryable<TDto>> GetQueryAsync<TDto>(params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        Task<IQueryable<TDto>> GetQueryAsync<TDto>(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        Task<IQueryable<TDto>> GetQueryAsync<TDto>(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TDto : class;
        Task<ISeries<TEntity>> HashMap<TDto>(IEnumerable<TDto> model, IEnumerable<TEntity> entity);
        Task<ISeries<TDto>> HashMap<TDto>(IEnumerable<TEntity> entity, IEnumerable<TDto> model);
        Task<ISeries<TEntity>> HashMapFrom<TDto>(IEnumerable<TDto> model);
        Task<ISeries<TDto>> HashMapTo<TDto>(IEnumerable<object> entity);
        Task<ISeries<TDto>> HashMapTo<TDto>(IEnumerable<TEntity> entity);
        void LinkTrigger(object sender, EntityEntryEventArgs e);
        Task<IList<TEntity>> Map<TDto>(IEnumerable<TDto> model, IEnumerable<TEntity> entity);
        Task<IList<TDto>> Map<TDto>(IEnumerable<TEntity> entity, IEnumerable<TDto> model);
        Task<TEntity> Map<TDto>(TDto model, TEntity entity);
        Task<TDto> Map<TDto>(TEntity entity, TDto model);
        Task<IList<TEntity>> MapFrom<TDto>(IEnumerable<TDto> model);
        Task<TDto> MapFrom<TDto>(object model);
        Task<TEntity> MapFrom<TDto>(TDto model);
        IAsyncEnumerable<TEntity> MapFromAsync<TDto>(IEnumerable<TDto> model);
        Task<IList<TDto>> MapTo<TDto>(IEnumerable<object> entity);
        Task<IList<TDto>> MapTo<TDto>(IEnumerable<TEntity> entity);
        Task<TDto> MapTo<TDto>(object entity);
        Task<TDto> MapTo<TDto>(TEntity entity);
        IAsyncEnumerable<TDto> MapToAsync<TDto>(IEnumerable<TEntity> entity);
        TEntity NewEntry(params object[] parameters);
        Task<bool> NotExist(Expression<Func<TEntity, bool>> predicate);
        Task<bool> NotExist(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<bool> NotExist(Type exceptionType, Expression<Func<TEntity, bool>> predicate, string message);
        Task<bool> NotExist(Type exceptionType, object instance, string message);
        Task<bool> NotExist<TException>(Expression<Func<TEntity, bool>> predicate, string message) where TException : Exception;
        Task<bool> NotExist<TException>(object instance, string message) where TException : Exception;
      
        Task<IPagedSet<TDto>> PagedFilter<TDto, TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IPagedSet<TDto>> PagedFilter<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IPagedSet<TDto>> PagedFilter<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IPagedSet<TDto>> PagedFilter<TDto, TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
       
        Task<IPagedSet<TResult>> PagedFilter<TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IPagedSet<TResult>> PagedFilter<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IPagedSet<TResult>> PagedFilter<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IPagedSet<TResult>> PagedFilter<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
     
        IEnumerable<TEntity> Patch<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IOrigin;
        IEnumerable<TEntity> Patch<TModel>(IEnumerable<TModel> entities, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IOrigin;
        Task<TEntity> Patch<TModel>(TModel delta) where TModel : class, IOrigin;
        Task<TEntity> Patch<TModel>(TModel delta, Func<TModel, Expression<Func<TEntity, bool>>> predicate) where TModel : class, IOrigin;
        Task<TEntity> Patch<TModel>(TModel delta, params object[] keys) where TModel : class, IOrigin;
        IAsyncEnumerable<TEntity> PatchAsync<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IOrigin;
        IAsyncEnumerable<TEntity> PatchAsync<TModel>(IEnumerable<TModel> entities, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IOrigin;
        IEnumerable<TEntity> PatchBy<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin;
        IEnumerable<TEntity> PatchBy<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin;
        Task<TEntity> PatchBy<TDto>(TDto model) where TDto : class, IOrigin;
        Task<TEntity> PatchBy<TDto>(TDto model, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin;
        Task<TEntity> PatchBy<TDto>(TDto model, params object[] keys) where TDto : class, IOrigin;
        IAsyncEnumerable<TEntity> PatchByAsync<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin;
        IAsyncEnumerable<TEntity> PatchByAsync<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate) where TDto : class, IOrigin;
        IEnumerable<TEntity> Put(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        Task<TEntity> Put(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        IAsyncEnumerable<TEntity> PutAsync(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        IEnumerable<TEntity> PutBy<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        Task<TEntity> PutBy<TDto>(TDto model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        IAsyncEnumerable<TEntity> PutByAsync<TDto>(IEnumerable<TDto> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        Task<IQueryable<TEntity>> QueryMapAsyncFrom<TDto>(IQueryable<TDto> model);
        Task<IQueryable<TDto>> QueryMapAsyncTo<TDto>(IQueryable<TEntity> entity) where TDto : class;
        IQueryable<TEntity> QueryMapFrom<TDto>(IQueryable<TDto> model);
        IQueryable<TDto> QueryMapTo<TDto>(IQueryable<TEntity> entity) where TDto : class;
        IEnumerable<TEntity> Set<TModel>(IEnumerable<TModel> models) where TModel : class, IOrigin;
        IEnumerable<TEntity> Set<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IOrigin;
        Task<TEntity> Set<TModel>(TModel entity) where TModel : class, IOrigin;
        Task<TEntity> Set<TModel>(TModel entity, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IOrigin;
        Task<TEntity> Set<TModel>(TModel entity, object key, Func<TEntity, Expression<Func<TEntity, bool>>> condition) where TModel : class, IOrigin;
        Task<TEntity> Set<TModel>(TModel entity, params object[] key) where TModel : class;
        IAsyncEnumerable<TEntity> SetAsync<TModel>(IEnumerable<TModel> models) where TModel : class, IOrigin;
        IAsyncEnumerable<TEntity> SetAsync<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IOrigin;
        IEnumerable<TEntity> SetBy<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin;
        IEnumerable<TEntity> SetBy<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin;
        Task<TEntity> SetBy<TDto>(TDto model) where TDto : class, IOrigin;
        Task<TEntity> SetBy<TDto>(TDto model, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin;
        Task<TEntity> SetBy<TDto>(TDto model, params object[] keys) where TDto : class, IOrigin;
        IAsyncEnumerable<TEntity> SetByAsync<TDto>(IEnumerable<TDto> entity) where TDto : class, IOrigin;
        IAsyncEnumerable<TEntity> SetByAsync<TDto>(IEnumerable<TDto> models, Func<TDto, Expression<Func<TEntity, bool>>> predicate, params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions) where TDto : class, IOrigin;
        TEntity Sign(TEntity entity);
        IQueryable<TEntity> Sort(IQueryable<TEntity> query, SortExpression<TEntity> sortTerms);
        TEntity Stamp(TEntity entity);
        TEntity Update(TEntity entity);
    }
}