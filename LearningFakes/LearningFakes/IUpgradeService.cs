using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearningFakes
{
    /// <summary>
    /// Check the current device version check if upgrades are required, check last upgrade date etc.
    /// </summary>
    public interface IUpgradeService
    {
        int currentSWVersion(int x);
        bool isSWUpgradeRequired(int DeviceID);
        DateTime lastUpgradeDate(int DeviceID);
        bool upgradeDevice(int DeviceID);


        IWDEImages Images { get; }
    }

    public interface IWDEImages
    {
        int Find(string ImageName);

    }
}
