using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotStatsTool.Services
{
    public class WpfLoadingVisualizationService : ILoadingVisualizationService
    {
        private readonly Action<double> SetProgress;

        public WpfLoadingVisualizationService(Action<double> setProgress)
        {
            SetProgress = setProgress;
        }

        void ILoadingVisualizationService.SetProgress(double progress)
        {
            SetProgress(progress);
        }
    }
}
