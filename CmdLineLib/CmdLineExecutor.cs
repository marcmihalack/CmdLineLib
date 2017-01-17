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

        object ProcessParameter(CmdLineParameter parameter, IDictionary<string, string> inputArgs)
        {
            var name = parameter.Name.ToLower();
            bool isArgProvided = inputArgs.TryGetValue(name, out string argValue);
            if (isArgProvided)
                inputArgs.Remove(name);
            return parameter.GetParameterValue(isArgProvided, argValue);
        }

        object Invoke(MethodDefinition method, IReadOnlyDictionary<string, string> inputArgs)
        {
            IDictionary<string, string> args = inputArgs.ToDictionary(p => p.Key, p => p.Value);
            //List<object> values = new List<object>();
            //List<CmdLineParameter> ps = new List<CmdLineParameter>();
            Dictionary<string, Tuple<string, bool>> argValues = new Dictionary<string, Tuple<string, bool>>();

            var ps = method.Parameters.Select(p =>
            {
                var name = p.Name.ToLower();
                bool isArgProvided = args.TryGetValue(name, out string argValue);
                if (isArgProvided)
                    args.Remove(name);
                return Tuple.Create(p, isArgProvided, argValue);
            }).ToArray();
            /*
            foreach (var parameter in method.Parameters)
            {
                var name = parameter.Name.ToLower();
                bool isArgProvided = args.TryGetValue(name, out string argValue);
                if (isArgProvided)
                    args.Remove(name);
                argValues.Add(name, Tuple.Create(argValue, isArgProvided));
                //var value = ProcessParameter(parameter, args);
                //values.Add(value);
            }*/

            if (args.Count > 0)
            {
                foreach (var item in Defintion.CommonParameters)
                {
                    var value = ProcessParameter(item, args);
                    item.SetValue(InstanceProvider.GetSafeInstance(), value);
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
