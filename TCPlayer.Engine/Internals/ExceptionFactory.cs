using ManagedBass;

namespace TCPlayer.Engine.Internals
{
    internal static class ExceptionFactory
    {
        public static EngineException Create(Errors errorCode, string message)
        {
            throw new EngineException($"{message}, code: {errorCode}");
        }
    }
}
