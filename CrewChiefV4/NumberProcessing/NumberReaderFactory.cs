﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrewChiefV4.NumberProcessing
{
    class NumberReaderFactory
    {
        private static String NUMBER_READER_IMPL_NAME = "NumberReaderImpl.cs";
        private static NumberReaderFactory INSTANCE = new NumberReaderFactory();
        private NumberReader numberReader;

        private NumberReaderFactory()
        {
            loadCode();
        }

        public static NumberReader GetNumberReader()
        {
            return INSTANCE.numberReader;
        }

        private Boolean LoadNumberReader(string code)
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            compilerparams.ReferencedAssemblies.Add(typeof(NumberReader).Assembly.Location);
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
                }
                Console.WriteLine(errors.ToString());
                return false;
            }
            else
            {
                this.numberReader = (NumberReader) results.CompiledAssembly.CreateInstance("CrewChiefV4.NumberProcessing.NumberReaderImpl");
                return true;
            }        
        }

        private void loadCode()
        {
            StreamReader file = null;
            Boolean loadedOverride = false;
            try
            {
                file = new StreamReader(Configuration.getUserOverridesFileLocation(NUMBER_READER_IMPL_NAME));
                StringBuilder sb = new StringBuilder();
                String line;
                while ((line = file.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }                
                LoadNumberReader(sb.ToString());
                loadedOverride = true;
            }
            catch (Exception) {}
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            if (!loadedOverride)
            {
                try
                {
                    file = new StreamReader(Configuration.getDefaultFileLocation(NUMBER_READER_IMPL_NAME));
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = file.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                    LoadNumberReader(sb.ToString());
                }
                catch (Exception e)
                {

                }
                finally
                {
                    if (file != null)
                    {
                        file.Close();
                    }
                }
            }
        }
    }
}
