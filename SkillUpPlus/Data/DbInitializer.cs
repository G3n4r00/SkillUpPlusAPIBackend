using SkillUpPlus.Models;

namespace SkillUpPlus.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Garante que o banco existe
            context.Database.EnsureCreated();

            // Se já existirem Tags, assumimos que o banco já foi populado
            if (context.InterestTags.Any())
            {
                return;
            }

            // 1. Tags de Interesse (IA, Soft Skills, etc.)
            var tags = new InterestTag[]
            {
                new InterestTag { Name = "Inteligência Artificial" },
                new InterestTag { Name = "Soft Skills" },
                new InterestTag { Name = "Sustentabilidade" },
                new InterestTag { Name = "Liderança" },
                new InterestTag { Name = "Programação" }
            };
            context.InterestTags.AddRange(tags);
            context.SaveChanges(); 

            // 2. Badges
            var badges = new Badge[]
            {
                new Badge { Name = "Pioneiro", IconUrl = "badge_pioneer.png" },
                new Badge { Name = "Mestre da Comunicação", IconUrl = "badge_comm.png" },
                new Badge { Name = "Futurista Tech", IconUrl = "badge_tech.png" }
            };
            context.Badges.AddRange(badges);
            context.SaveChanges();

            // 3. Trilhas e Módulos
            var tracks = new Track[]
            {
                // Trilha 1: Soft Skills
                new Track
                {
                    Title = "Comunicação Não-Violenta",
                    Description = "Aprenda técnicas para se comunicar de forma mais empática, evitando conflitos desnecessários.",
                    Category = "Soft Skills",
                    EstimatedTimeMinutes = 120,
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "Introdução à CNV",
                            Content = "A Comunicação Não-Violenta (CNV) é uma abordagem que...",
                            ModuleType = "text"
                        },
                        new Module
                        {
                            Title = "Os 4 Componentes",
                            Content = "Observação, Sentimento, Necessidade e Pedido.",
                            ModuleType = "text"
                        },
                        new Module
                        {
                            Title = "Quiz de Fixação",
                            Content = "Qual o primeiro passo da CNV?",
                            ModuleType = "quiz"
                        }
                    }
                },
                // Trilha 2: IA
                new Track
                {
                    Title = "Inteligência Artificial para Todos",
                    Description = "Entenda como a IA está mudando o mercado de trabalho e como se adaptar.",
                    Category = "Inteligência Artificial",
                    EstimatedTimeMinutes = 180,
                    Modules = new List<Module>
                    {
                        new Module
                        {
                            Title = "História da IA",
                            Content = "Desde Turing até os LLMs modernos...",
                            ModuleType = "text"
                        },
                        new Module
                        {
                            Title = "IA Generativa na Prática",
                            Content = "Como usar prompts efetivos.",
                            ModuleType = "video"
                        }
                    }
                }
            };

            context.Tracks.AddRange(tracks);
            context.SaveChanges();
        }
    }
}
