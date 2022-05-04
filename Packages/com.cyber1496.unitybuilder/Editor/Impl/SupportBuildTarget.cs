using System;

namespace UnityBuilder {
    [Flags]
    public enum SupportBuildTarget
    {
        StandaloneOSX = 0b_000_000_0000_0001,
        StandaloneWindows = 0b_000_000_0000_0010,
        StandaloneWindows64 = 0b_000_000_0000_0100,
        StandaloneLinux64 = 0b_000_000_0000_1000,
        iOS = 0b_000_000_0001_0000,
        Android = 0b_000_000_0010_0000,

		Everything = 0b_1111_1111_1111_1111,
    }
}