using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotStatsTool.Services
{
    public interface ILoadingVisualizationService
    {
        void SetProgress(double progress);
    }
}
