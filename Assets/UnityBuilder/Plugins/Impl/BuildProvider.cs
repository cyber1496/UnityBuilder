using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityBuilder {
    using UnityEditor.Build.Reporting;
    using StandardKit;
    public enum RegisterState : int {
        Success, Duplicated,
    }
    public static class BuildProvider {
        static IBuildHelper buildeHelper = new BuildHelper();
        static IBuildLogHandler buildLogHandler = new BuildLogHandler();
        static IProcessor processor = new Processor();
        static readonly SortedDictionary<int, List<IPreProcessor>> preProcessores = new SortedDictionary<int, List<IPreProcessor>>();
        static readonly SortedDictionary<int, List<IPostProcessor>> postProcessores = new SortedDictionary<int, List<IPostProcessor>>();

        public static RegisterState RegisterBuildHelper(IBuildHelper newBuildHelper) {
            var state = buildeHelper is BuildHelper || newBuildHelper is BuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            buildeHelper = newBuildHelper;
            return state;
        }
        public static RegisterState RegisterBuildLogHandler(IBuildLogHandler newBuildLogHandler) {
            var state = buildeHelper is BuildLogHandler || newBuildLogHandler is BuildLogHandler ? RegisterState.Success : RegisterState.Duplicated;
            buildLogHandler = newBuildLogHandler;
            return state;
        }
        public static RegisterState RegisterProcessor(IProcessor newProcessor) {
            var state = processor is Processor || newProcessor is BuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            processor = newProcessor;
            return state;
        }
        public static RegisterState RegisterPreProcessor(IPreProcessor preProcessor) {
            var state = preProcessores.ContainsKey(preProcessor.PreOrder) ? RegisterState.Duplicated : RegisterState.Success;
            if (!preProcessores.ContainsKey(preProcessor.PreOrder)) {
                preProcessores.Add(preProcessor.PreOrder, new List<IPreProcessor>());
            }
            preProcessores[preProcessor.PreOrder].Add(preProcessor);
            return state;
        }
        public static RegisterState RegisterPostProcessor(IPostProcessor postProcessor) {
            var state = postProcessores.ContainsKey(postProcessor.PostOrder) ? RegisterState.Duplicated : RegisterState.Success;
            if (!postProcessores.ContainsKey(postProcessor.PostOrder)) {
                postProcessores.Add(postProcessor.PostOrder, new List<IPostProcessor>());
            }
            postProcessores[postProcessor.PostOrder].Add(postProcessor);
            return state;
        }
        public static void PreProcess() {
            Array.ForEach(preProcessores.Values.ToArray(), procList => Array.ForEach(procList.ToArray(), proc => proc?.PreProcess(buildeHelper)));
        }
        public static void PostProcess() {
            Array.ForEach(postProcessores.Values.ToArray(), procList => Array.ForEach(procList.ToArray(), proc => proc?.PostProcess(buildeHelper)));
        }
        public static void Process() {
            buildLogHandler.PreProcess(buildeHelper);
            PreProcess();
            var result = processor.Process(buildeHelper);
            PostProcess();
            buildLogHandler.PostProcess(buildeHelper);
        }
    }
}