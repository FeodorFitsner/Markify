﻿using System;
using System.Linq;

using Optional;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public sealed class SolutionExplorer : ISolutionExplorer
    {
        #region Fields

        private readonly IIDEEnvironment _ideEnv;

        #endregion

        #region Properties

        public Option<Solution> CurrentSolution
        {
            get
            {
                var name = _ideEnv.CurrentSolution;
                if (name == null)
                    return Option.None<Solution>();

                return new Solution
                (
                    _ideEnv.CurrentSolution,
                    _ideEnv.GetSolutionPath(name),
                    _ideEnv.GetProjects(name)
                ).Some();
            }
        }

        public Option<string> CurrentProject
        {
            get
            {
                return _ideEnv.CurrentProject != null ? 
                    _ideEnv.CurrentProject.Some() :
                    Option.None<string>();
            }
        }

        #endregion

        #region Constructors

        public SolutionExplorer(IIDEEnvironment ideEnv)
        {
            _ideEnv = ideEnv;
        }

        #endregion

        #region Methods

        public Option<Project> GetProject(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return default(Option<Project>);

            if (!_ideEnv.GetProjects(_ideEnv.CurrentSolution).Contains(name))
                return default(Option<Project>);

            var projectPath = _ideEnv.GetProjectPath(name);
            var solutionPath = _ideEnv.GetSolutionPath(_ideEnv.CurrentSolution);
            if (projectPath == null || solutionPath == null)
                return default(Option<Project>);

            return new Project
            ( 
                name,
                projectPath ?? new Uri(solutionPath, name),
                _ideEnv.GetProjectFiles(name) ?? new Uri[0]
            ).Some();
        }

        #endregion
    }
}
