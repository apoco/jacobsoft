using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMoq;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd.Test
{
    public class MockServiceLocator : IServiceLocator
    {
        private readonly AutoMoqer mocker;

        public MockServiceLocator(AutoMoqer mocker)
        {
            this.mocker = mocker;
        }

        public T Get<T>() where T : class
        {
            return mocker.GetMock<T>().Object;
        }
    }
}
