using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Threading
{
    internal class ProcessThreading
    {
        // these moved here in an attemps to find a pattern and create only one delegate signature
        public delegate int NoiseFactory_Del_Process(
                    Engine.Surface.Canvas image,
                    Engine.Effects.Noise.IModule module,
                    int yStart,
                    int yEnd
            );

        public delegate int ScannerRadial_Del_Process(
                    int yStart,
                    int yEnd);

        public delegate int DelegatedMethod(int start, int end, ParamList lst);
    }

    internal class ThreadedLoop : IDisposable
    {
        // TODO : find ref for that
        // setting these as member of the class prevents GC to delete them prematurely
        private IAsyncResult[] cookies;
        private Engine.Threading.ProcessThreading.DelegatedMethod[] dels;


        // TODO implement Dispose() ?
        public ThreadedLoop()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">The number of items to iterate through within the loop</param>
        public void Loop(int length, Engine.Threading.ProcessThreading.DelegatedMethod method, ParamList lst)
        {
            int workerThreads = 0;
            int completionPortsThreads = 0;

            // workerThreads or completionPortsThreads are supposed to return the same value as the number of cores in computer.
            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionPortsThreads);

            // with max number of cores available, don't divide the work too much
            int divide = Engine.Calc.Math.Threading_Division(length, System.Math.Min(workerThreads, completionPortsThreads));
            int[] starts = Engine.Calc.Math.Threading_GetStarts(length, divide);
            int[] lengths = Engine.Calc.Math.Threading_GetLengths(length, divide);

            dels = new Engine.Threading.ProcessThreading.DelegatedMethod[divide];
            cookies = new IAsyncResult[divide];

            for (int i = 0; i < divide; i++)
            {
                dels[i] = method;
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i] = dels[i].BeginInvoke(starts[i], lengths[i], lst, null, null);
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i].AsyncWaitHandle.WaitOne();
            }

            for (int i = 0; i < divide; i++)
            {
                int result = dels[i].EndInvoke(cookies[i]);
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i].AsyncWaitHandle.Close();
            }
        }

        #region Dispose
        // for details see D:\Docs\My Projects\bmp_creator\Dispose which leads to http://dave-black.blogspot.ca/2011/03/how-do-you-properly-implement.html

        // remember to make this class inherit from IDisposable -> MyDisposableClass : IDisposable

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Default initialization for a bool is 'false'</remarks>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// Implementation of Dispose according to .NET Framework Design Guidelines.
        /// </summary>
        /// <remarks>Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.

            // Always use SuppressFinalize() in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overloaded Implementation of Dispose.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// <list type="bulleted">Dispose(bool isDisposing) executes in two distinct scenarios.
        /// <item>If <paramref name="isDisposing"/> equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.</item>
        /// <item>If <paramref name="isDisposing"/> equals <c>false</c>, the method has been called 
        /// by the runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</item></list>
        /// </remarks>
        protected virtual void Dispose(bool isDisposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            try
            {
                if (!this.IsDisposed)
                {
                    // Explicitly set root references to null to expressly tell the GarbageCollector
                    // that the resources have been disposed of and it's ok to release the memory 
                    // allocated for them.
                    if (isDisposing)
                    {
                        // Release all managed resources here
                        // Need to unregister/detach yourself from the events. Always make sure
                        // the object is not null first before trying to unregister/detach them!
                        // Failure to unregister can be a BIG source of memory leaks
                        /*if (someDisposableObjectWithAnEventHandler != null)
                        {
                            someDisposableObjectWithAnEventHandler.SomeEvent -= someDelegate;
                            someDisposableObjectWithAnEventHandler.Dispose();
                            someDisposableObjectWithAnEventHandler = null;
                        }*/

                        if (dels != null)
                        {
                            for (int i = 0; i < dels.Length; i++)
                            {
                                dels[i] = null;
                            }
                        }

                        if (cookies != null)
                        {
                            for (int i = 0; i < cookies.Length; i++)
                            {
                                cookies[i] = null;
                            }
                        }

                        // If this is a WinForm/UI control, uncomment this code
                        //if (components != null)
                        //{
                        //    components.Dispose();
                        //}
                    }
                    // Release all unmanaged resources here  
                    // (example)             if (someComObject != null && Marshal.IsComObject(someComObject))
                    //{
                    //    Marshal.FinalReleaseComObject(someComObject);
                    //    someComObject = null;
                    //}
                }
            }
            finally
            {
                this.IsDisposed = true;
            }
        }

        //TODO Uncomment this code if this class will contain members which are UNmanaged
        ///// <summary>Finalizer for MyDisposableClass</summary>
        ///// <remarks>This finalizer will run only if the Dispose method does not get called.
        ///// It gives your base class the opportunity to finalize.
        ///// DO NOT provide finalizers in types derived from this class.
        ///// All code executed within a Finalizer MUST be thread-safe!</remarks>
        //  ~MyDisposableClass()
        //  {
        //     Dispose( false );
        //  }
        #endregion
    }

    internal class ParamList
    {
        Dictionary<string, KeyValuePair<Type, object>> t_params;

        public ParamList()
        {
            t_params = new Dictionary<string, KeyValuePair<Type, object>>();
        }

        public void Add(string name, Type @type)
        {
            t_params.Add(name, new KeyValuePair<Type, object>(type, null));
        }

        public void Add(string name, Type @type, object @value)
        {
            t_params.Add(name, new KeyValuePair<Type, object>(type, value));
        }

        public KeyValuePair<Type, object> Get(string name)
        {
            return t_params[name];
        }
    }
}
