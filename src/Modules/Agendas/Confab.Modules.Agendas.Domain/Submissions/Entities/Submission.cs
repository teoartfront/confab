﻿using System.Collections.Generic;
using System.Linq;
using Confab.Modules.Agendas.Domain.Submissions.Constants;
using Confab.Modules.Agendas.Domain.Submissions.Events;
using Confab.Modules.Agendas.Domain.Submissions.Exceptions;
using Confab.Shared.Abstractions.Kernel.Types;

namespace Confab.Modules.Agendas.Domain.Submissions.Entities
{
    internal sealed class Submission : AggregateRoot
    {
        private IEnumerable<Speaker> _speakers;

        public Submission(AggregateId id, ConferenceId conferenceId, string title, string description, int level,
            string status, IEnumerable<string> tags, ICollection<Speaker> speakers, int version = 0)
            : this(id, conferenceId)
        {
            Title = title;
            Description = description;
            Level = level;
            Status = status;
            Tags = tags;
            _speakers = speakers;
            Version = version;
        }

        public Submission(AggregateId id, ConferenceId conferenceId)
        {
            (Id, ConferenceId) = (id, conferenceId);
        }

        public ConferenceId ConferenceId { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Level { get; private set; }
        public string Status { get; set; }
        public IEnumerable<string> Tags { get; private set; }
        public IEnumerable<Speaker> Speakers => _speakers;

        public static Submission Create(AggregateId id, ConferenceId conferenceId, string title, string description,
            int level, IEnumerable<string> tags, ICollection<Speaker> speakers)
        {
            var submission = new Submission(id, conferenceId);
            submission.ChangeTitle(title);
            submission.ChangeDescription(description);
            submission.ChangeLevel(level);
            submission.Status = SubmissionStatus.Pending;
            submission.Tags = tags;
            submission.ChangeSpeakers(speakers);

            submission.ClearEvents();
            submission.AddEvent(new SubmissionAdded(submission));

            return submission;
        }

        public void ChangeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new EmptySubmissionTitleException(Id);

            Title = title;
            IncrementVersion();
        }

        public void ChangeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new EmptySubmissionDescriptionException(Id);

            Description = description;
            IncrementVersion();
        }

        public void ChangeLevel(int level)
        {
            var isNotInRange = level is < 1 or > 6;
            if (isNotInRange)
                throw new InvalidSubmissionLevelException(Id);

            Level = level;
            IncrementVersion();
        }

        public void ChangeSpeakers(IEnumerable<Speaker> speakers)
        {
            if (speakers is null || !speakers.Any())
                throw new MissingSubmissionSpeakersException(Id);

            _speakers = speakers;
            IncrementVersion();
        }

        public void Approve()
        {
            if (Status is SubmissionStatus.Rejected)
                throw new InvalidSubmissionStatusException(Id, SubmissionStatus.Approved, Status);

            Status = SubmissionStatus.Approved;
            AddEvent(new SubmissionApproved(Id));
        }

        public void Reject()
        {
            if (Status is SubmissionStatus.Approved)
                throw new InvalidSubmissionStatusException(Id, SubmissionStatus.Rejected, Status);

            Status = SubmissionStatus.Rejected;
            AddEvent(new SubmissionRejected(Id));
        }
    }
}