﻿using System;
using System.Collections.Generic;
using Confab.Modules.Agendas.Domain.Submissions.Exceptions;
using Confab.Shared.Abstractions.Kernel.Types;

namespace Confab.Modules.Agendas.Domain.Submissions.Entities
{
    public class Speaker : AggregateRoot
    {
        public string FullName { get; private set; }
        public ICollection<Submission> Submissions { get; }

        public static Speaker Create(Guid id, string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new EmptyFullnameOfSpeakerException(id);

            return new Speaker
            {
                Id = id,
                FullName = fullName
            };
        }
    }
}