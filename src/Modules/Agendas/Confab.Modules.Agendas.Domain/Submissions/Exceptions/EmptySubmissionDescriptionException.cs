﻿using System;
using Confab.Shared.Abstractions.Exceptions;

namespace Confab.Modules.Agendas.Domain.Submissions.Exceptions
{
    internal sealed class EmptySubmissionDescriptionException : ConfabException
    {
        public EmptySubmissionDescriptionException(Guid submissionId)
            : base($"Submission with ID: '{submissionId}' defines empty description.")
        {
            SubmissionId = submissionId;
        }

        public Guid SubmissionId { get; }
    }
}