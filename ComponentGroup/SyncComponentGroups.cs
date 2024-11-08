using SyncingDOTSandMono.SyncPlayerLoop;
using Unity.Entities;

namespace SyncingDOTSandMono.SyncComponentGroup
{
    [UpdateInGroup(typeof(SyncBeforeMonoUpdate))]
    public partial class SyncBeforeMonoGroup : ComponentSystemGroup { }
    [UpdateInGroup(typeof(SyncAfterMonoUpdate))]
    public partial class SyncAfterMonoGroup : ComponentSystemGroup { }
}