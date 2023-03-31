using NexenHub.Class;

namespace NexenHub.ViewModels
{
    public class MachineTagnameViewModel
    {

        public MachineBasicInfo MachineInfo { get; set; }

        public MachineTagnameViewModel(MachineBasicInfo info)
        {
            MachineInfo = info;
        }
    }
}
