using CmdLineLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CmdLineLib
{
    class CmdLineExecutor
    {
        Type Type;
        object Instance;
        IInstanceProvider InstanceProvider;
        ClassDefinition Defintion;

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public CmdLineExecutor(ClassDefinition classDefinition, IInstanceProvider instanceProvider)
        {
            Defintion = classDefinition ?? throw new ArgumentNullException(nameof(classDefinition));
            InstanceProvider = instanceProvider;
            Type = instanceProvider.TypeOf;
            Instance = instanceProvider.Instance;
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public bool Execute(string command, IReadOnlyDictionary<string, string> args, out object returnValue)
        {
            var methods = Defintion.FindMethods(command);
            if (methods.Length > 1)
            {
                int i = 0;
                CmdLineArgException lastEx = null;
                do
                {
                    try
                    {
                        returnValue = Invoke(methods[i], args);
                        return true;
                    }
                    catch (CmdLineArgException ex)
                    {
                        lastEx = ex;
                    }
                }
                while (++i < methods.Length);
                throw lastEx;
                // TODO: method help
                //returnValue = null;
                //return false;
            }
            else if (methods.Length == 1)
            {
                returnValue = Invoke(methods[0], args);
                return true;
            }
            else
            {
                returnValue = null;
                return false;
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        Tuple<CmdLineParameter, bool, string> ProcessParameter(CmdLineParameter parameter, IDictionary<string, string> args)
        {
            var name = parameter.Name;
            var pair = args.Where(p1 => p1.Key.Equals(name, Defintion.Config.NameComparison)).FirstOrDefault();
            if (pair.Key != null)
                args.Remove(pair.Key);
            return Tuple.Create(parameter, pair.Key != null, pair.Value);
        }

        object Invoke(MethodDefinition method, IReadOnlyDictionary<string, string> inputArgs)
        {
            IDictionary<string, string> args = inputArgs.ToDictionary(p => p.Key, p => p.Value);
            //List<object> values = new List<object>();
            //List<CmdLineParameter> ps = new List<CmdLineParameter>();

            var ps = method.Parameters.Select(p => ProcessParameter(p, args)).ToArray();

            if (args.Count > 0)
            {
                foreach (var item in Defintion.CommonParameters)
                {
                    var t3 = ProcessParameter(item, args);
                    if(t3.Item2)
                        item.SetValue(InstanceProvider.GetSafeInstance(), item.GetParameterValue(t3.Item2, t3.Item3));
                }

                if (args.Count > 0)
                {
                    var name = args.First().Key;
                    throw new CmdLineArgException(name, $"Unknown argument '{name}'");
                }
            }
            var values = ps.Select(t3 => t3.Item1.GetParameterValue(t3.Item2, t3.Item3)).ToArray();
            var instance = method.Method.IsStatic ? null : (Instance ?? InstanceProvider.GetSafeInstance());
            return method.Method.Invoke(instance, values);
        }
    }
}
