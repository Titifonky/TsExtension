using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LogDebugging
{
    [Flags]
    public enum LogLevelL4N
    {
        DEBUG = 1,
        ERROR = 2,
        FATAL = 4,
        INFO = 8,
        WARN = 16
    }

    public static class Log
    {
        private static readonly ILog _Logger = LogManager.GetLogger("DLL");

        private static Boolean _EstInitialise = false;

        private static Boolean _Actif = true;

        public static String Chemin = "";

        static Log()
        {
            String Dossier = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Log)).Location);
            Chemin = Path.Combine(Dossier, "log4net.config");
            XmlConfigurator.Configure(_Logger.Logger.Repository, new FileInfo(Chemin));
        }

        public static void Demarrer()
        {
            Activer = true;
            Entete();
        }

        public static void Stopper()
        {
            IAppender[] appenders = _Logger.Logger.Repository.GetAppenders();
            foreach (IAppender appender in appenders)
            {
                appender.Close();
            }
            _Logger.Logger.Repository.Shutdown();
        }

        public static void Entete()
        {
            if (_EstInitialise)
                return;

            Write("\n ");
            Write("====================================================================================================");
            Write("|                                                                                                  |");
            Write("|                                          DEBUG                                                   |");
            Write("|                                                                                                  |");
            Write("====================================================================================================");
            Write("\n ");

            _EstInitialise = true;
        }

        public static Boolean Activer
        {
            get
            {
                return _Actif;
            }
            set
            {
                _Actif = value;

                log4net.Core.Level pLevel = log4net.Core.Level.Debug;
                if (value)
                    pLevel = log4net.Core.Level.All;

                ILoggerRepository repository = _Logger.Logger.Repository;
                repository.Threshold = pLevel;

                ((log4net.Repository.Hierarchy.Logger)_Logger.Logger).Level = pLevel;

                log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)repository;
                log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
                rootLogger.Level = pLevel;

            }
        }

        public static void Write(Object Message, LogLevelL4N Level = LogLevelL4N.DEBUG)
        {
            try
            {
                if (Level.Equals(LogLevelL4N.DEBUG))
                    _Logger.Debug(Message.ToString());
                else if (Level.Equals(LogLevelL4N.ERROR))
                    _Logger.Error(Message.ToString());
                else if (Level.Equals(LogLevelL4N.FATAL))
                    _Logger.Fatal(Message.ToString());
                else if (Level.Equals(LogLevelL4N.INFO))
                    _Logger.Info(Message.ToString());
                else if (Level.Equals(LogLevelL4N.WARN))
                    _Logger.Warn(Message.ToString());
            }
            catch { }
        }

        public static void Message(Object message)
        {
            if (!_Actif)
                return;

            if(message != null)
                Write("\t\t\t\t-> " + message.ToString());
            else
                Write("\t\t\t\t-> null");
        }

        public static void LogMethode(this Object O, Object Message, [CallerMemberName] String methode = "")
        {
            Methode(O.GetType().Name, Message, methode);
        }

        public static void LogMethode(this Object O, [CallerMemberName] String methode = "")
        {
            Methode(O.GetType().Name, methode);
        }

        public static void Methode(String nomClasse, [CallerMemberName] String methode = "")
        {
            if (!_Actif)
                return;

            Write("\t\t\t" + nomClasse + "." + methode);
        }

        public static void Methode(String nomClasse, Object message, [CallerMemberName] String methode = "")
        {
            if (!_Actif)
                return;

            Write("\t\t\t" + nomClasse + "." + methode);
            if(message != null)
                Write("\t\t\t\t-> " + message.ToString());
        }

        public static void Methode<T>([CallerMemberName] String methode = "")
        {
            Methode(typeof(T).ToString(), methode);
        }

        public static void Methode<T>(Object message, [CallerMemberName] String methode = "")
        {
            Methode(typeof(T).ToString(), message, methode);
        }
    }
}

