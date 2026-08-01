# Arcadia Online — Starter Project (Fase 1: Prototipe)

Starter kit ini dibuat berdasarkan dokumentasi resmi di `Arcadia-Online-Docs`
(`docs/02_TDD/*`, `docs/01_GDD/*`). Isinya adalah **struktur folder + script
skeleton yang sudah compile-ready**, mengikuti arsitektur, naming convention,
dan coding standard yang sudah kamu tetapkan di dokumen.

Ini BUKAN file project Unity utuh (tidak ada `ProjectSettings/`, `.sln`, dll)
karena file-file itu wajib digenerate oleh Unity Editor sendiri. Ikuti langkah
di bawah untuk menggabungkannya ke project Unity yang baru.

## Yang sudah dibuat

| Folder | Isi |
|---|---|
| `Assets/Scripts/Core/` | `Singleton`, `GameManager`, `Events` (event system global), `ObjectPool`, `StateMachine`/`IState` |
| `Assets/Scripts/Player/` | `PlayerController`, `PlayerStats` (implements `IDamageable`), `PlayerInventory`, `PlayerEquipment` |
| `Assets/Scripts/Combat/` | `CombatManager`, `TargetLockSystem`, `DamageCalculator`, `SkillSystem`, `SkillData` (ScriptableObject), `IDamageable` |
| `Assets/Scripts/CameraSystem/` *(folder fisik: `Camera/`)* | `CameraController`, `CameraCollision`, `CameraShake` |
| `Assets/Scripts/InputHandling/` | `InputManager` (pakai Unity Input System) |
| `Assets/Scripts/Save/` | `SaveManager`, `SaveData`, `AutoSaveSystem` |
| `Assets/Scripts/Managers/` | `AudioManager`, `UIManager`, `QuestManager`, `WorldStateManager` |
| `Assets/Scripts/Data/` | `StatBlock`, `JobData` (ScriptableObject) |
| `Assets/Scripts/Monster/` | `MonsterStats`, `MonsterAI` (state machine Patrol→Chase→Attack) |
| `Assets/Scripts/Utils/` | `MathHelper`, `Extensions` |
| `Assets/InputActions/ArcadiaControls.inputactions` | Binding keyboard+gamepad sesuai `docs/02_TDD/Input.md` |
| `Packages/manifest.json` | Daftar package yang dibutuhkan |

Semua script mengikuti standar di `docs/02_TDD/CodingStandard.md`:
PascalCase untuk class/method, `_camelCase` untuk private field, namespace
`ArcadiaOnline.<Module>`.

## Langkah Setup (Unity 6 LTS)

1. **Buat project baru** di Unity Hub: pilih template **3D (URP)**, nama
   project `Arcadia-Online`, Unity 6 LTS (6000.0.x).
2. **Tutup Unity**, lalu salin isi folder `Assets/` dari starter kit ini
   (script, InputActions) ke `Assets/` project barumu — timpa/gabung folder
   yang sama.
3. Salin juga `.gitignore` dan `.gitattributes` ke root project (untuk
   Git + Git LFS sesuai `docs/02_TDD/Architecture.md`).
4. **Buka Unity lagi.** Editor akan otomatis compile script dan resolve
   package dari `Packages/manifest.json`. Ini butuh koneksi internet.
   > Catatan: versi package di `manifest.json` adalah estimasi untuk Unity 6
   > LTS. Jika ada versi yang tidak cocok, buka **Window > Package Manager**
   > dan biarkan Unity menyarankan versi kompatibel — hapus baris versi yang
   > error dan install ulang lewat UI.
5. **Generate C# class dari Input Actions:**
   - Klik `Assets/InputActions/ArcadiaControls.inputactions`
   - Di Inspector, centang **"Generate C# Class"** lalu **Apply**
   - Ini diperlukan supaya `InputManager.cs` bisa kompilasi bersih
     (saat ini `InputManager` membaca action lewat `PlayerInput` component,
     jadi generate class bersifat opsional tapi direkomendasikan untuk
     type-safety).
6. **Setup GameObject dasar di scene:**
   - Buat GameObject kosong `_Managers`, tempelkan: `GameManager`,
     `SaveManager`, `AudioManager`, `UIManager`, `QuestManager`,
     `WorldStateManager`, `AutoSaveSystem` — set `DontDestroyOnLoad` (sudah
     built-in via `Singleton<T>`).
   - Buat GameObject `Player`: tambahkan `CharacterController`,
     `PlayerInput` (assign `ArcadiaControls.inputactions`, default map
     `Player`), `PlayerController`, `PlayerStats`, `PlayerInventory`,
     `PlayerEquipment`, `TargetLockSystem`, `SkillSystem`.
   - Buat GameObject `InputManager` (atau tempel di Player), pasang
     komponen `InputManager` + `PlayerInput`.
   - Buat `Main Camera` sebagai child dari GameObject kosong `CameraRig`,
     tempelkan `CameraController` + `CameraCollision` + `CameraShake`,
     assign `_target` ke Player.
7. **Buat data asset pertama:**
   - `Assets > Create > Arcadia > Job Data` — buat 3 asset: `Warrior`,
     `Mage`, `Archer`. Isi `startingStats` sesuai tabel di
     `docs/01_GDD/04_Player.md`:

     | Stat | Warrior | Mage | Archer |
     |---|---|---|---|
     | HP | 120 | 80 | 100 |
     | MP | 30 | 100 | 50 |
     | ATK | 15 | 8 | 12 |
     | DEF | 12 | 5 | 8 |
     | MATK | 5 | 15 | 6 |
     | SPD | 8 | 7 | 12 |

   - `Assets > Create > Arcadia > Skill Data` — buat skill awal sesuai
     tabel di `docs/01_GDD/07_Skills.md` (contoh: `Slash`, `Fire Bolt`).
8. **NavMesh untuk monster:** install package `AI Navigation`
   (sudah ada di manifest), lalu bake NavMesh di scene dungeon contoh
   sebelum `MonsterAI` bisa bergerak.

## Yang belum diimplementasikan (sengaja, sesuai scope Fase 1)

Beberapa method sengaja dibiarkan `TODO` karena butuh keputusan desain
lanjutan atau aset (UI prefab, drop table, dialog data) yang belum ada:
- `SaveManager.CollectSaveData()` / `ApplySaveData()` — perlu dihubungkan
  ke semua sistem begitu strukturnya stabil.
- `CombatManager.ExecuteSkill()` — perlu keputusan bagaimana efek buff/debuff
  diterapkan (lihat `docs/01_GDD/07_Skills.md`).
- `MonsterStats.Die()` — masih pakai `Destroy()`, sebaiknya diganti
  `ObjectPool<T>.Return()` untuk performa (lihat `docs/02_TDD/Optimization.md`
  yang belum kita baca detail).
- `UIManager` — semua method menunggu prefab UI aktual (lihat `Art_Bible`
  untuk gaya visual dan `docs/01_GDD/26_UI.md` untuk layout).

## Referensi silang ke dokumentasi

Setiap file script punya komentar `<summary>` yang merujuk ke dokumen GDD/TDD
sumbernya. Kalau ada perubahan desain, update dokumen dulu, baru sinkronkan
ke script — bukan sebaliknya (sesuai filosofi "dokumentasi = fondasi resmi").
