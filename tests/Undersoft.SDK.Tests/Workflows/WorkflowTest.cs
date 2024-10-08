namespace Undersoft.SDK.Tests.Workflows
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;
    using Undersoft.SDK.Invoking;
    using Undersoft.SDK.Tests.Workflows.Features;
    using Undersoft.SDK.Workflows;

    [TestClass]
    public class WorkflowTest
    {
        public WorkflowTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        public void Workflow_MultiThreading_ParallelConcurrentSynchronization_Framework_Integration_Test()
        {
            var work = new Workflow();

            var download = work
                .Aspect<BankCurrencyService>()
                    .AddWork<FirstCurrency>((w) => w.GetCurrency)
                    .AddWork<SecondCurrency>((w) => w.GetCurrency)
                .Allocate(6);

            var compute = work
                .Aspect<WorkflowTest>()
                    .AddWork<ComputeCurrency>((w) => w.Compute)
                    .AddWork<PresentResult>((w) => w.Present)
                .Allocate(3);

            download
                .Work<FirstCurrency>((w) => w.GetCurrency)
                    .FlowTo<ComputeCurrency>((w) => w.Compute)
                .Work<SecondCurrency>((w) => w.GetCurrency)
                    .FlowTo<ComputeCurrency>((w) => w.Compute);

            compute
                .Work<PresentResult>((w) => w.Present)
                    .FlowFrom<ComputeCurrency>((w) => w.Compute, (r) => (double)((object[])r)[2] > 0.8950D);

            for (int i = 1; i < 7; i++)
            {
                download
                    .Work<FirstCurrency>((w) => w.GetCurrency).Post("EUR", i)
                    .Work<SecondCurrency>((w) => w.GetCurrency).Post("USD", i);
            }

            Task.Delay(15000).Wait();

            download.Close(true);
            compute.Close(true);
        }

        [TestMethod]
        public void Workout_Integration_Test()
        {
            var ql0 = new Work<FirstCurrency>("EUR", 1);
            var ql1 = new Work<SecondCurrency>("USD", 1);

            var ql2 = Work.Run<FirstCurrency>(true, "EUR", 1);
            var ql3 = Work.Run<SecondCurrency>(true, "USD", 1);            

            Task.Delay(5000).Wait();
        }
    }
}
