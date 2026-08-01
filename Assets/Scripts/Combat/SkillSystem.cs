using UnityEngine;

namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Maks 8 skill yang di-equip sekaligus. Lihat docs/01_GDD/07_Skills.md
    /// dan docs/02_TDD/CombatArchitecture.md.
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        public const int MAX_EQUIPPED_SKILLS = 8;

        [SerializeField] private SkillData[] _equippedSkills = new SkillData[MAX_EQUIPPED_SKILLS];
        private float[] _cooldownTimers = new float[MAX_EQUIPPED_SKILLS];

        private void Update()
        {
            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                if (_cooldownTimers[i] > 0f)
                {
                    _cooldownTimers[i] -= Time.deltaTime;
                }
            }
        }

        public bool CanUseSkill(int index)
        {
            if (index < 0 || index >= _equippedSkills.Length) return false;
            if (_equippedSkills[index] == null) return false;
            return _cooldownTimers[index] <= 0f;
        }

        public SkillData UseSkill(int index)
        {
            if (!CanUseSkill(index)) return null;

            SkillData skill = _equippedSkills[index];
            _cooldownTimers[index] = skill.cooldown;
            return skill;
        }

        public float GetCooldownRemaining(int index)
        {
            if (index < 0 || index >= _cooldownTimers.Length) return 0f;
            return Mathf.Max(0f, _cooldownTimers[index]);
        }

        public void EquipSkill(SkillData skill, int slot)
        {
            if (slot < 0 || slot >= _equippedSkills.Length) return;
            _equippedSkills[slot] = skill;
            _cooldownTimers[slot] = 0f;
        }

        public void UnequipSkill(int slot)
        {
            if (slot < 0 || slot >= _equippedSkills.Length) return;
            _equippedSkills[slot] = null;
        }

        public SkillData GetSkill(int index)
        {
            if (index < 0 || index >= _equippedSkills.Length) return null;
            return _equippedSkills[index];
        }
    }
}
