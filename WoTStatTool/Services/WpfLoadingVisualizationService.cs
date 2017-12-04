using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotStatsTool.Services
{
    public class WpfLoadingVisualizationService : ILoadingVisualizationService
    {
        private readonly Action<int> SetProgress;

        public WpfLoadingVisualizationService(Action<int> setProgress)
        {
            SetProgress = setProgress;
        }

        void ILoadingVisualizationService.SetProgress(double progress)
        {
            SetProgress((int)(progress * 100));
        }
    }
}
