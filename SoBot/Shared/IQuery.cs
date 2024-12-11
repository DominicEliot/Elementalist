// Copyright (c) Dominic Eliot.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SorceryBot.Shared;
public interface IQuery<TResult> : IRequest<TResult>
{
}
