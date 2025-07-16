using System.Collections.Generic;
using Assets.Scripts.Events;

namespace Assets.Scripts.Triggers
{
    public class TriggerCheckContext
    {
        private List<TriggerEvent> _triggersToCheck;

        public bool IsValid;
        public List<TriggerEvent> TriggersToCheck => _triggersToCheck;

        public TriggerCheckContext(List<TriggerEvent> triggersToCheck)
        {
            _triggersToCheck = triggersToCheck;
            IsValid = false;
        }
    }
}