using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Buat particle effect sederhana secara programmatic.
    /// Attach ke GameObject kosong, lalu panggil CreateXXXEffect().
    /// </summary>
    public class SimpleVFXCreator : MonoBehaviour
    {
        /// <summary>
        /// Buat hit effect (spark/flash kuning).
        /// </summary>
        public static GameObject CreateHitEffect()
        {
            GameObject effect = new GameObject("HitEffect");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();

            // Stop dulu sebelum set properties
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.3f;
            main.startLifetime = 0.3f;
            main.startSpeed = 5f;
            main.startSize = 0.2f;
            main.startColor = new Color(1f, 0.8f, 0f); // Kuning
            main.maxParticles = 20;
            main.loop = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 15)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.1f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.yellow, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;

            // Auto destroy
            Destroy(effect, 1f);

            return effect;
        }

        /// <summary>
        /// Buat death effect (asap merah/abu-abu).
        /// </summary>
        public static GameObject CreateDeathEffect()
        {
            GameObject effect = new GameObject("DeathEffect");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();

            // Stop dulu sebelum set properties
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.startLifetime = 1.5f; // Lebih lama
            main.startSpeed = 3f; // Lebih cepat
            main.startSize = 0.8f; // Lebih besar
            main.startColor = new Color(0.8f, 0.1f, 0.1f); // Merah lebih terang
            main.maxParticles = 50; // Lebih banyak
            main.loop = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 25)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.5f, 0.1f, 0.1f), 0f),
                    new GradientColorKey(Color.black, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, 2f);

            Destroy(effect, 2f);

            return effect;
        }

        /// <summary>
        /// Buat skill effect (lingkaran biru/cyan).
        /// </summary>
        public static GameObject CreateSkillEffect()
        {
            GameObject effect = new GameObject("SkillEffect");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();

            // Stop dulu sebelum set properties
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.startLifetime = 0.8f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = new Color(0f, 0.8f, 1f); // Cyan
            main.maxParticles = 40;
            main.loop = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 30)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.cyan, 0f),
                    new GradientColorKey(Color.blue, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;

            Destroy(effect, 1.5f);

            return effect;
        }

        /// <summary>
        /// Buat heal effect (lingkaran hijau naik ke atas).
        /// </summary>
        public static GameObject CreateHealEffect()
        {
            GameObject effect = new GameObject("HealEffect");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();

            // Stop dulu sebelum set properties
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.2f;
            main.startColor = new Color(0f, 1f, 0.3f); // Hijau
            main.maxParticles = 20;
            main.loop = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 15)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.3f;

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(2f, 4f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.green, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;

            Destroy(effect, 1.5f);

            return effect;
        }

        /// <summary>
        /// Buat level up effect (spiral emas naik ke atas).
        /// </summary>
        public static GameObject CreateLevelUpEffect()
        {
            GameObject effect = new GameObject("LevelUpEffect");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();

            // Stop dulu sebelum set properties
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 1f;
            main.startLifetime = 1.5f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = new Color(1f, 0.8f, 0f); // Emas
            main.maxParticles = 50;
            main.loop = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 40)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(3f, 5f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.yellow, 0f),
                    new GradientColorKey(Color.white, 0.5f),
                    new GradientColorKey(Color.yellow, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = gradient;

            Destroy(effect, 2f);

            return effect;
        }
    }
}
