using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Globals;
namespace LogicLayer
{
    public class Logic : ILogic
    {
        private IDice dice;
        private CancellationTokenSource cts;
        private Task job;

        public Logic(IDice dice)
        {
            this.dice = dice;
        }

        public Dictionary<int, long> Values
        {
            get
            {
                return Values;
            }
        }

        public event Action<long> ValuesChanged;

        public void Calculate(CancellationToken token, IProgress<long> progress)
        {

            if (token.IsCancellationRequested)
            {

               
            }
        }

        public void Start()
        {
            var progress = new Progress<long>((result)=>{
               ValuesChanged?.Invoke(result);
           });
            cts = new CancellationTokenSource();
            job = Task.Run(() => { Calculate( cts.Token, progress); });

        }

        public void Stop()
        {
            cts.Cancel();
            
        }
    }
}
