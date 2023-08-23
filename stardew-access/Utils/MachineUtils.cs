using StardewValley.Objects;

namespace stardew_access.Utils
{
    internal static class MachineUtils
    {
        internal enum MachineState
        {
            Ready,
            Busy,
            Waiting
        }

        /// <summary>
        /// Gets the machine state based on the provided machine object.
        /// </summary>
        /// <param name="machine">The machine object to analyze.</param>
        /// <returns>The state of the machine.</returns>
        internal static MachineState GetMachineState(StardewValley.Object machine)
        {
            if (machine is CrabPot crabPot)
            {
                bool hasBait = crabPot.bait.Value is not null;
                bool hasHeldObject = crabPot.heldObject.Value is not null;
                if (hasBait && !hasHeldObject)
                    return MachineState.Busy;
                else if (hasBait && hasHeldObject)
                    return MachineState.Ready;
            }
            return GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, machine.heldObject.Value);
        }
        /// <summary>
        /// Gets the machine state based on provided parameters.
        /// </summary>
        /// <param name="readyForHarvest">Is the machine ready for harvest?</param>
        /// <param name="minutesUntilReady">Minutes until the machine is ready.</param>
        /// <param name="heldObject">The held object within the machine.</param>
        /// <returns>The state of the machine.</returns>
        internal static MachineState GetMachineState(bool readyForHarvest, int minutesUntilReady, StardewValley.Object? heldObject)
        {
            if (readyForHarvest || (heldObject is not null && minutesUntilReady <= 0))
            {
                return MachineState.Ready;
            }
            else if (minutesUntilReady > 0)
            {
                return MachineState.Busy;
            }
            else
            {
                return MachineState.Waiting;
            }
        }
    }
}
