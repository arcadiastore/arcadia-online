# Arcadia Online — Unity 6 LTS RPG Project

> **3D Cel-Shaded Anime MMORPG** — Story-driven, Open World, Offline First

---

## 📁 Struktur Repository

```
arcadia-online/
├── docs/                    ← Semua dokumentasi (GDD, TDD, DDD, dll)
│   ├── 01_GDD/              ← Game Design Document
│   ├── 02_TDD/              ← Technical Design Document
│   ├── 03_DDD/              ← Data Design Document
│   ├── 04_Story_Bible/      ← World Building & Lore
│   └── 05_Art_Bible/        ← Visual Design & Style Guide
├── Assets/                  ← Unity Project
│   └── Scripts/
│       ├── Core/            ← GameManager, Events, Singleton
│       ├── Player/          ← PlayerController, PlayerStats, dll
│       ├── Camera/          ← CameraController, CameraCollision
│       ├── Combat/          ← CombatManager, TargetLock, SkillSystem
│       ├── Monster/         ← MonsterAI, MonsterStats
│       ├── Save/            ← SaveManager, SaveData, AutoSave
│       ├── Managers/        ← UIManager, AudioManager, QuestManager
│       ├── InputHandling/   ← InputManager (Unity Input System)
│       ├── Data/            ← JobData, StatBlock
│       └── Utils/           ← Extensions, MathHelper
├── Packages/                ← Unity Package Manifest
├── PROGRESS.md              ← Tracking kemajuan pengembangan
├── ROADMAP.md               ← Rencana pengembangan
└── CHANGELOG.md             ← Catatan perubahan
```

---

## 🚀 Status Saat Ini

| Fase | Status | Progress |
|------|--------|----------|
| Fase 0: Dokumentasi | ✅ SELESAI | 100% |
| **Fase 1: Prototipe** | 🔄 DALAM PROSES | **30%** |
| Fase 2: Vertical Slice | ⏳ BELUM MULAI | 0% |

---

## 📖 Dokumentasi

Semua dokumen dalam **Bahasa Indonesia**:

| Dokumen | Lokasi | Isi |
|---------|--------|-----|
| Game Design | `docs/01_GDD/` | Visi, gameplay, sistem, konten |
| Technical Design | `docs/02_TDD/` | Arsitektur, folder structure, coding standard |
| Data Design | `docs/03_DDD/` | Database schema, data flow |
| Story Bible | `docs/04_Story_Bible/` | World building, karakter, lore |
| Art Bible | `docs/05_Art_Bible/` | Visual style, referensi, asset list |

---

## 🎮 Yang Sudah Dibuat (Fase 1 Scripts)

| Modul | Script | Fungsi |
|-------|--------|--------|
| **Core** | GameManager | Game state, pause, play time |
| | Events | Event system global |
| | Singleton | Base class singleton |
| | StateMachine | AI state management |
| | ObjectPool | Object pooling |
| **Player** | PlayerController | WASD + Sprint + Stamina |
| | PlayerStats | HP, MP, Level, EXP |
| | PlayerInventory | Inventory dasar |
| | PlayerEquipment | Equipment slot |
| **Camera** | CameraController | Third person follow |
| | CameraCollision | Hindari tembus dinding |
| | CameraShake | Efek shake |
| **Combat** | CombatManager | Sistem pertarungan |
| | TargetLockSystem | Kunci target |
| | SkillSystem | Sistem skill |
| | DamageCalculator | Hitung damage |
| **Monster** | MonsterAI | AI patrol/chase/attack |
| | MonsterStats | Stat monster |
| **Save** | SaveManager | Save/Load JSON |
| | SaveData | Data structure |
| | AutoSaveSystem | Auto save |
| **Managers** | UIManager | UI management |
| | AudioManager | Audio management |
| | QuestManager | Quest system |
| | WorldStateManager | WorldState flags |
| **Input** | InputManager | Unity Input System |
| **Data** | JobData | Job ScriptableObject |
| | StatBlock | Stat block |
| **Utils** | Extensions | Extension methods |
| | MathHelper | Math utility |

---

## 🛠 Setup Unity

```bash
# 1. Clone repo
git clone https://github.com/arcadiastore/arcadia-online.git

# 2. Buka Unity Hub → Add → pilih folder arcadia-online

# 3. Buka project (Unity 6 LTS)

# 4. Generate Input Actions:
#    - Klik Assets/InputActions/ArcadiaControls.inputactions
#    - Centang "Generate C# Class" → Apply

# 5. Setup scene test:
#    - Player: GameObject + PlayerController + PlayerStats
#    - Camera: Main Camera + CameraController
#    - Ground: 3D Object → Plane
#    - Managers: GameObject + GameManager
```

---

## 📋 Referensi

| Topik | File |
|-------|------|
| Folder Structure Unity | `docs/02_TDD/FolderStructure.md` |
| Coding Standard | `docs/02_TDD/CodingStandard.md` |
| Game Loop | `docs/01_GDD/03_GameLoop.md` |
| Combat System | `docs/01_GDD/05_Combat.md` |
| Player Stats | `docs/01_GDD/04_Player.md` |
| Skill System | `docs/01_GDD/07_Skills.md` |
| UI Layout | `docs/01_GDD/26_UI.md` |

---

## 📝 License

MIT License — Lihat [LICENSE](LICENSE)

---

## 🔗 Links

- **GitHub**: https://github.com/arcadiastore/arcadia-online
- **Issues**: https://github.com/arcadiastore/arcadia-online/issues
