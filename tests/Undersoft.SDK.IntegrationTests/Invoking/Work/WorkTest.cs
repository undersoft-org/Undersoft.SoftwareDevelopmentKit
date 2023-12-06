namespace Undersoft.SDK.IntegrationTests.Invoking.Work
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using Undersoft.SDK.Invoking;
    using Undersoft.SDK.Invoking.Work;
    using Xunit;

    public class WorkTest
    {
        public WorkTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void Workflow_MultiThreading_Integration_Test()
        {
            var work = new Workflow<WorkAspects>();

            var download = work
                .Aspect<WorkTest>()
                    .AddWork<FirstCurrency>()
                    .AddWork<SecondCurrency>()
                        .Allocate(4);

            var compute = work
                .Aspect<NBPSource>()
                    .AddWork<ComputeCurrency>()
                    .AddWork<PresentResult>()
                        .Allocate(2);

            download
                .Work<FirstCurrency>()
                    .FlowTo<ComputeCurrency>()
                .Work<SecondCurrency>()
                    .FlowTo<ComputeCurrency>();

            compute
                .Work<PresentResult>()
                    .FlowFrom<ComputeCurrency>();

            for (int i = 1; i < 10; i++)
            {
                download
                    .Work<FirstCurrency>()
                        .Run("EUR", i)
                    .Work<SecondCurrency>()
                        .Run("USD", i);
            }

            Thread.Sleep(10000);

            download.Close(true);
            compute.Close(true);
        }

        [Fact]
        public void Workout_Integration_Test()
        {
            var ql0 = new Workout(new Invoker<FirstCurrency>(), "EUR", 1);
            var ql1 = new Workout(new Invoker<SecondCurrency>(), "USD", 1);

            ql0 = Workout.Run<FirstCurrency>(true, "EUR", 1);
            ql1 = Workout.Run<SecondCurrency>(false, "USD", 1);

            Thread.Sleep(5000);
        }
    }

    public class NBPSource
    {
        private const string file_dir = "http://www.nbp.pl/Kursy/xml/dir.txt";
        private const string xml_url = "http://www.nbp.pl/kursy/xml/";

        public string file_name;
        public DateTime rate_date;
        private int start_int = 1;

        public NBPSource(int daysbefore)
        {
            GetFileName(daysbefore);
        }

        public Dictionary<string, double> GetCurrenciesRate(List<string> currency_names)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            foreach (var item in currency_names)
            {
                result.Add(item, LoadRate(item));
            }
            return result;
        }

        public double LoadRate(string currency_name)
        {
            try
            {
                string file = xml_url + file_name + ".xml";
                DataSet ds = new DataSet();
                ds.ReadXml(file);
                var tabledate = ds.Tables["tabela_kursow"].Rows.Cast<DataRow>().AsEnumerable();
                var before_rate_date = (
                    from k in tabledate
                    select new { Data = k["data_publikacji"].ToString() }
                ).First();
                var tabela = ds.Tables["pozycja"].Rows.Cast<DataRow>().AsEnumerable();
                var rate = (
                    from k in tabela
                    where k["kod_waluty"].ToString() == currency_name
                    select new { Kurs = k["kurs_sredni"].ToString() }
                ).First();
                rate_date = Convert.ToDateTime(before_rate_date.Data);
                return Convert.ToDouble(rate.Kurs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }

        private async void GetFileName(int daysbefore)
        {
            try
            {
                int minusdays = daysbefore * -1;
                HttpClient client = new HttpClient();
                string file_list = await client.GetStringAsync(file_dir);
                string date_str = string.Empty;
                DateTime date_of_rate = DateTime.Now.AddDays(minusdays);
                int maxdaysbackward = 3;
                while (true)
                {
                    date_str =
                        "a"
                        + start_int.ToString().PadLeft(3, '0')
                        + "z"
                        + date_of_rate.ToString("yyMMdd");
                    if (file_list.Contains(date_str))
                        break;

                    start_int++;

                    if (start_int > 365)
                    {
                        if (--maxdaysbackward < 1)
                            throw new ArgumentOutOfRangeException();

                        start_int = 1;
                        date_of_rate = date_of_rate.AddDays(-1);
                    }
                }
                file_name = date_str;
                rate_date = date_of_rate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }
    }

    public class ComputeCurrency
    {
        public object Compute(string currency1, double rate1, string currency2, double rate2)
        {
            try
            {
                double _rate1 = rate1;
                double _rate2 = rate2;
                double result = _rate2 / _rate1;
                Debug.WriteLine("Result : " + result.ToString());

                return new object[] { _rate1, _rate2, result };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }
    }

    public class FirstCurrency
    {
        public object GetCurrency(string currency, int days)
        {
            NBPSource kurKraju = new NBPSource(days);

            try
            {
                double rate = kurKraju.LoadRate(currency);
                Debug.WriteLine(
                    "Rate 1 : "
                        + currency
                        + "  days past: "
                        + days.ToString()
                        + " amount : "
                        + rate.ToString("#.####")
                );

                return new object[] { currency, rate };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }
    }

    public class PresentResult
    {
        public object Present(double rate1, double rate2, double result)
        {
            try
            {
                string present =
                    "Rate USD : "
                    + rate1.ToString()
                    + " EUR : "
                    + rate2.ToString()
                    + " EUR->USD : "
                    + result.ToString();
                Debug.WriteLine(present);
                return present;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }
    }

    public class SecondCurrency
    {
        public object GetCurrency(string currency, int days)
        {
            NBPSource kurKraju = new NBPSource(days);

            try
            {
                double rate = kurKraju.LoadRate(currency);
                Debug.WriteLine(
                    "Rate 2 : "
                        + currency
                        + " days past : "
                        + days.ToString()
                        + " amount : "
                        + rate.ToString("#.####")
                );

                return new object[] { currency, rate };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new InvalidWorkException(ex.ToString());
            }
        }
    }
}
