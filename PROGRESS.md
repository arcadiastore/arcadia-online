# Progress Tracking — Arcadia Online

> File ini berisi catatan kemajuan pengembangan. Update setiap kali ada perubahan.

---

## Status Saat Ini

| Fase | Status | Progress |
|------|--------|----------|
| Fase 0: Dokumentasi | ✅ SELESAI | 100% |
| Fase 1: Prototipe | 🔄 DALAM PROSES | 90% |
| Fase 2: Vertical Slice | ⏳ BELUM MULAI | 0% |

---

## Fase 1: Prototipe — Checklist

### Script Sudah Dibuat
- [x] GameManager.cs (Core)
- [x] Events.cs (Event System)
- [x] Singleton.cs (Base class)
- [x] StateMachine.cs (AI state)
- [x] ObjectPool.cs (Performance)
- [x] PlayerController.cs (Movement WASD + Sprint)
- [x] PlayerStats.cs (HP, MP, Level, EXP)
- [x] PlayerInventory.cs (Inventory dasar)
- [x] PlayerEquipment.cs (Equipment slot)
- [x] CameraController.cs (Third person follow)
- [x] CameraCollision.cs (Hindari tembus dinding)
- [x] CameraShake.cs (Efek shake)
- [x] CombatManager.cs (Sistem pertarungan)
- [x] TargetLockSystem.cs (Kunci target)
- [x] SkillSystem.cs (Sistem skill)
- [x] DamageCalculator.cs (Hitung damage)
- [x] IDamageable.cs (Interface)
- [x] SkillData.cs (Data skill)
- [x] MonsterAI.cs (AI monster)
- [x] MonsterStats.cs (Stat monster)
- [x] SaveManager.cs (Save/Load)
- [x] SaveData.cs (Data structure)
- [x] AutoSaveSystem.cs (Auto save)
- [x] UIManager.cs (UI management)
- [x] AudioManager.cs (Audio management)
- [x] QuestManager.cs (Quest system)
- [x] WorldStateManager.cs (WorldState)
- [x] InputManager.cs (Input handling)
- [x] JobData.cs (Job data)
- [x] StatBlock.cs (Stat block)
- [x] Extensions.cs (Utility)
- [x] MathHelper.cs (Math utility)
- [x] ArcadiaControls.inputactions (Input actions)

### Folder Structure (Sesuai TDD)
- [x] Assets/Scripts/Core/
- [x] Assets/Scripts/Player/
- [x] Assets/Scripts/Camera/
- [x] Assets/Scripts/Combat/
- [x] Assets/Scripts/Monster/
- [x] Assets/Scripts/Save/
- [x] Assets/Scripts/Managers/
- [x] Assets/Scripts/InputHandling/
- [x] Assets/Scripts/Data/
- [x] Assets/Scripts/Utils/
- [x] Assets/Art/ (subfolders ready)
- [x] Assets/Audio/
- [x] Assets/Data/
- [x] Assets/Prefabs/
- [x] Assets/Scenes/
- [x] Assets/UI/
- [x] Assets/Resources/
- [x] Assets/Plugins/

### Setup Unity (Dilakukan User)
- [x] Install Unity 6 LTS (6000.5.6f1)
- [x] Buka project dari GitHub
- [x] Fix package issues (hapus Cinemachine, Burst)
- [x] Setup URP (Universal Render Pipeline)
- [x] Setup scene: Player, Camera, Ground, GameManager
- [x] Test: Player bergerak WASD (camera-relative)
- [x] Test: Camera follow + mouse rotate
- [x] Test: Sprint + stamina
- [x] Test: HP/MP bar di UI

### Belum Dibuat (Next Steps)
- [x] Prefab Monster (test dummy)
- [x] Scene Fase 1 (test arena)
- [x] UI Prefab (HUD) - SimpleHUD
- [x] Sound Effect (SFX) - Male/Female per action
- [x] BGM System - Map + Battle
- [ ] Material cel-shader dasar
- [ ] Animasi dasar (idle, walk, run, attack)
- [ ] Particle effect (hit, death)
- [ ] Monster AI (patrol, chase)

---

## Repository

| Repo | URL | Isi |
|------|-----|-----|
| **Docs** | https://github.com/arcadiastore/Arcadia-Online-Docs | Dokumentasi V1.0 |
| **Unity** | https://github.com/arcadiastore/arcadia-online | Project Unity |

---

## Catatan Perubahan

### 2026-08-01 (Update 2)
- **Unity setup selesai** — Unity 6 LTS (6000.5.6f1) ter-install & jalan
- **URP aktif** — Universal Render Pipeline ter-install
- **Package cleanup** — Hapus Cinemachine, Burst (broke compatibility)
- **Input System aktif** — Both mode (lama + baru)
- **Scene test jalan** — Player bergerak WASD, Camera follow mouse
- **SimplePlayerController** — Camera-relative movement working
- **Fase 1 progress**: 50%

### 2026-08-01 (Awal)
- **Dokumentasi V1.0 selesai** — 95+ dokumen terpush ke GitHub
- **Unity project starter dibuat** — 33 scripts terpush ke GitHub
- **1 repository gabungan**: arcadia-online (docs + Unity)

### Langkah Selanjutnya
1. User install Unity 6 LTS di PC
2. Clone repo `arcadia-online` ke PC
3. Buka project di Unity
4. Setup scene test (Player + Camera + Ground)
5. Test semua script
6. Lanjut ke sistem pertarungan (Combat)
