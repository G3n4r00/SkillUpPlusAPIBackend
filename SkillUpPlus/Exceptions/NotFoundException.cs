namespace SkillUpPlus.Exceptions
{
    /// <summary>
    /// Exceção customizada para ser usada quando um recurso (usuário, trilha)
    /// não é encontrado. O Middleware vai traduzir isso para um HTTP 404.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
