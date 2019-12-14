using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityBuilder {
    public enum RegisterState : int {
        Success, Duplicated,
    }
    public static class BuildProvider {
        static IBuildHelper buildeHelper = new StandardBuildHelper();
        static IBuildLogHandler buildLogHandler = new StandardBuildLogHandler();
        static IProcessor processor = new StandardProcessor();
        static readonly SortedDictionary<int, IPreProcessor> preProcessores = new SortedDictionary<int, IPreProcessor>();
        static readonly SortedDictionary<int, IPostProcessor> postProcessores = new SortedDictionary<int, IPostProcessor>();

        public static RegisterState RegisterBuildHelper(IBuildHelper newBuildHelper) {
            var state = buildeHelper is StandardBuildHelper || newBuildHelper is StandardBuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            buildeHelper = newBuildHelper;
            return state;
        }
        public static RegisterState RegisterBuildLogHandler(IBuildLogHandler newBuildLogHandler) {
            var state = buildeHelper is StandardBuildLogHandler || newBuildLogHandler is StandardBuildLogHandler ? RegisterState.Success : RegisterState.Duplicated;
            buildLogHandler = newBuildLogHandler;
            return state;
        }
        public static RegisterState RegisterProcessor(IProcessor newProcessor) {
            var state = processor is StandardProcessor || newProcessor is StandardBuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            processor = newProcessor;
            return state;
        }
        public static RegisterState RegisterPreProcessor(IPreProcessor preProcessor) {
            var state = preProcessores.ContainsKey(preProcessor.PreOrder) ? RegisterState.Duplicated : RegisterState.Success;
            preProcessores.Add(preProcessor.PreOrder, preProcessor);
            return state;
        }
        public static RegisterState RegisterPostProcessor(IPostProcessor postProcessor) {
            var state = postProcessores.ContainsKey(postProcessor.PostOrder) ? RegisterState.Duplicated : RegisterState.Success;
            postProcessores.Add(postProcessor.PostOrder, postProcessor);
            return state;
        }
        public static void Process() {
            buildLogHandler.PreProcess();
            Array.ForEach(preProcessores.Values.ToArray(), proc => proc?.PreProcess());
            processor.Process();
            Array.ForEach(postProcessores.Values.ToArray(), proc => proc?.PostProcess());
            buildLogHandler.PostProcess();
        }
    }
}