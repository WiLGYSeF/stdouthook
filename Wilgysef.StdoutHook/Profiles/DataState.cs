﻿using System;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class DataState
    {
        // TODO: not null?
        public string? Data { get; internal set; }

        public bool Stdout { get; }

        public ProfileState ProfileState { get; }

        public RuleContext Context { get; private set; } = new RuleContext();

        public DataState(string data, bool stdout, ProfileState profileState)
        {
            Data = data;
            Stdout = stdout;
            ProfileState = profileState;
        }

        public DataState(ProfileState profileState)
        {
            ProfileState = profileState;
        }

        public IDisposable GetContextScope()
        {
            return new ContextReset(this);
        }

        private class ContextReset : IDisposable
        {
            private readonly DataState _dataState;

            public ContextReset(DataState dataState)
            {
                _dataState = dataState;
            }

            public void Dispose()
            {
                _dataState.Context = new RuleContext();
            }
        }
    }
}