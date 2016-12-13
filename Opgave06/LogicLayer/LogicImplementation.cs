using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
using System.Threading;

namespace LogicLayer
{
    public class LogicImplementation : ILogic

    {
        private CancellationTokenSource cts;
        private Task search;
        private int interval;
        private int passLength;
        private DateTime previousTime;

        public LogicImplementation()
        {
            interval = 50;
        }
        public int ReportInterval
        {
            set
            {
                interval = value;
            }
        }

        public event Action<string> CollisionFound;
        public event Action<ulong, int> ProgressChanged;

        public void AbortSearch()
        {
           
            cts.Cancel();
        }

        public void StartSearchCollisionsTask(Sha1Hash hash)
        {
            var progress = new Progress<ulong>((count) =>
            {
               
                
                ProgressChanged?.Invoke(count,passLength);

                
            });
            var collisionFound = new Progress<string>((result) =>
            {
                CollisionFound?.Invoke(result);
            });
            cts = new CancellationTokenSource();
            search = Task.Run(() => { SearchCollisions(hash, progress, collisionFound, cts.Token); });
            
        }

        public void SearchCollisions(Sha1Hash hash, IProgress<ulong> progress, IProgress<string> collisionFound, CancellationToken cancelToken)
        {
            var generator = new PasswordGenerator();
            ulong count = 0;
            previousTime = DateTime.Now;


            foreach (var pass in generator)
            {
                count++;
                passLength = pass.Length;
                if (Sha1Hash.CalculateFromString(pass) == hash)
                {

                    collisionFound.Report(pass);

                }
                if ((DateTime.Now - previousTime).TotalMilliseconds >= interval)
                {
                    progress?.Report(count);
                    previousTime = DateTime.Now;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    progress?.Report(count);
                    break;
                }



            }
        }
    }
}
