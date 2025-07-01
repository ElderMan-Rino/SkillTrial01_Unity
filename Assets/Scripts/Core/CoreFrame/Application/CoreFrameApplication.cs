using Elder.Core.Common.BaseClasses;
using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using System.Collections.Generic;
using System;

namespace Elder.Core.CoreFrame.Application
{
    public class CoreFrameApplication : DisposableBase, IApplicationProvider
    {
        private Dictionary<Type, IApplication> _persistentApp;

        /*
         * 먼저 _persistentApp 추가
         * 1. 로거부터 추가 
         * 2. 이벤트 버스 
         * 
         * 로거, 이벤트 버스 먼저 추가
         * 여기에 Logger가지고 오는 인터페이스도ㅁ 추가
         * 
         * 이벤트 버스 Creator도 추가해야하나?
         */


        public bool TryGetApplication<T>(out T targetApplication) where T : IApplication
        {
            throw new NotImplementedException();
        }

        protected override void DisposeManagedResources()
        {

        }

        protected override void DisposeUnmanagedResources()
        {

        }
    }
}
