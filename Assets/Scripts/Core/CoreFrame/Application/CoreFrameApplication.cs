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
         * ���� _persistentApp �߰�
         * 1. �ΰź��� �߰� 
         * 2. �̺�Ʈ ���� 
         * 
         * �ΰ�, �̺�Ʈ ���� ���� �߰�
         * ���⿡ Logger������ ���� �������̽����� �߰�
         * 
         * �̺�Ʈ ���� Creator�� �߰��ؾ��ϳ�?
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
