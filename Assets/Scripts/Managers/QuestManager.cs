using System.Collections.Generic;
using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Managers
{
    /// <summary>Lihat docs/02_TDD/GameManagers.md dan docs/01_GDD/14_Quest.md.</summary>
    public class QuestManager : Singleton<QuestManager>
    {
        private readonly List<string> _activeQuests = new List<string>();
        private readonly HashSet<string> _completedQuests = new HashSet<string>();
        private readonly HashSet<string> _failedQuests = new HashSet<string>();

        public void AcceptQuest(string questId)
        {
            if (_activeQuests.Contains(questId)) return;
            _activeQuests.Add(questId);
            Events.QuestAccepted(questId);
        }

        public void CompleteQuest(string questId)
        {
            if (!_activeQuests.Remove(questId)) return;
            _completedQuests.Add(questId);
            Events.QuestComplete(questId);
        }

        public void FailQuest(string questId)
        {
            if (!_activeQuests.Remove(questId)) return;
            _failedQuests.Add(questId);
            Events.QuestFailed(questId);
        }

        public List<string> GetActiveQuests() => new List<string>(_activeQuests);

        public bool IsQuestComplete(string questId) => _completedQuests.Contains(questId);
    }
}
