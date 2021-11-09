using System.CommandLine;
using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Facts.Testing;
using LanceC.Tooling.DevOps.Properties;
using Moq.AutoMock;
using Octokit;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.GitHub;

public class ReleasableIssueConversionServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly DeployOptions _options = new();

    private ReleasableIssueConversionService CreateSystemUnderTest(
        DeployOptions? options = default,
        StubConsole? stubConsole = default)
    {
        _mocker.Use(options ?? _options);
        _mocker.Use<IConsole>(stubConsole ?? new());
        return _mocker.CreateInstance<ReleasableIssueConversionService>();
    }

    public class TheConvertAllMethod : ReleasableIssueConversionServiceFacts
    {
        [Fact]
        public void ReturnsReleasableIssuesFromResolutionAndType()
        {
            // Arrange
            var firstIssueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var firstIssueType = BuiltInIssueTypes.Bug;
            var firstIssue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(firstIssueResolution.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(firstIssueType.Name, _options),
                });

            var secondIssueResolution = BuiltInIssueResolutions.Completed;
            var secondIssueType = BuiltInIssueTypes.Feature;
            var secondIssue = GitHubStubCreator.CreateIssue(
                "Bar",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(secondIssueResolution.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(secondIssueType.Name, _options),
                });

            var sut = CreateSystemUnderTest();

            // Act
            var releasableIssues = sut.ConvertAll(new[] { firstIssue, secondIssue, });

            // Assert
            Assert.Equal(2, releasableIssues.Count);
            Assert.Single(
                releasableIssues,
                releasableIssue =>
                    releasableIssue.Number == firstIssue.Number &&
                    releasableIssue.Title == firstIssue.Title &&
                    releasableIssue.Resolution == firstIssueResolution &&
                    releasableIssue.Type == firstIssueType);
            Assert.Single(
                releasableIssues,
                releasableIssue =>
                    releasableIssue.Number == secondIssue.Number &&
                    releasableIssue.Title == secondIssue.Title &&
                    releasableIssue.Resolution == secondIssueResolution &&
                    releasableIssue.Type == secondIssueType);
        }

        [Fact]
        public void IgnoresLabelsWithoutPrefixSeparator()
        {
            // Arrange
            var issueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var issueType = BuiltInIssueTypes.Bug;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateLabel("Bar"),
                    GitHubStubCreator.CreateResolutionLabel(issueResolution.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(issueType.Name, _options),
                });

            var sut = CreateSystemUnderTest();

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(issueResolution, releasableIssue.Resolution);
            Assert.Equal(issueType, releasableIssue.Type);
        }

        [Fact]
        public void UsesResolutionLabelClassificationFromOptions()
        {
            // Arrange
            var newLabelClassification = new LabelClassification("Baz", "baz");
            var options = new DeployOptions
            {
                IssueResolutionLabelClassification = newLabelClassification,
            };
            options.LabelClassifications.Add(newLabelClassification);

            var issueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var issueType = BuiltInIssueTypes.Bug;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(issueResolution.Name, options),
                    GitHubStubCreator.CreateTypeLabel(issueType.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(issueResolution, releasableIssue.Resolution);
            Assert.Equal(issueType, releasableIssue.Type);
        }

        [Fact]
        public void UsesTypeLabelClassificationFromOptions()
        {
            // Arrange
            var newLabelClassification = new LabelClassification("Baz", "baz");
            var options = new DeployOptions
            {
                IssueTypeLabelClassification = newLabelClassification,
            };
            options.LabelClassifications.Add(newLabelClassification);

            var issueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var issueType = BuiltInIssueTypes.Bug;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(issueResolution.Name, options),
                    GitHubStubCreator.CreateTypeLabel(issueType.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(issueResolution, releasableIssue.Resolution);
            Assert.Equal(issueType, releasableIssue.Type);
        }

        [Fact]
        public void UsesLabelPrefixSeparatorFromOptions()
        {
            // Arrange
            var options = new DeployOptions
            {
                LabelPrefixSeparator = "-",
            };

            var issueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var issueType = BuiltInIssueTypes.Bug;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(issueResolution.Name, options),
                    GitHubStubCreator.CreateTypeLabel(issueType.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(issueResolution, releasableIssue.Resolution);
            Assert.Equal(issueType, releasableIssue.Type);
        }

        [Fact]
        public void UsesIssueResolutionsFromOptions()
        {
            // Arrange
            var newIssueResolution = new IssueResolution("Baz", false);
            var options = new DeployOptions();
            options.IssueResolutions.Add(newIssueResolution);

            var issueType = BuiltInIssueTypes.Bug;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(newIssueResolution.Name, options),
                    GitHubStubCreator.CreateTypeLabel(issueType.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(newIssueResolution, releasableIssue.Resolution);
            Assert.Equal(issueType, releasableIssue.Type);
        }

        [Fact]
        public void UsesIssueTypesFromOptions()
        {
            // Arrange
            var newIssueType = new IssueType("Baz", false);
            var options = new DeployOptions();
            options.IssueTypes.Add(newIssueType);

            var issueResolution = BuiltInIssueResolutions.ByDesignWontFix;
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(issueResolution.Name, options),
                    GitHubStubCreator.CreateTypeLabel(newIssueType.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var releasableIssues = sut.ConvertAll(new[] { issue, });

            // Assert
            var releasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(issue.Number, releasableIssue.Number);
            Assert.Equal(issue.Title, releasableIssue.Title);
            Assert.Equal(issueResolution, releasableIssue.Resolution);
            Assert.Equal(newIssueType, releasableIssue.Type);
        }

        [Fact]
        public void ReturnsEmptyArrayWhenIssueCollectionIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var releasableIssues = sut.ConvertAll(default!);

            // Assert
            Assert.Empty(releasableIssues);
        }

        [Fact]
        public void ReturnsEmptyArrayWhenIssueCollectionIsEmpty()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var releasableIssues = sut.ConvertAll(Array.Empty<Issue>());

            // Assert
            Assert.Empty(releasableIssues);
        }

        [Fact]
        public void SkipsIssueThatHasNoResolutionLabel()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleasableIssueLabelMissingFormat(issue.Number, "resolution")));
        }

        [Fact]
        public void SkipsIssueThatHasMultipleResolutionLabels()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, _options),
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.Completed.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(
                error => error.Write(Messages.ReleasableIssueLabelDuplicateFormat(issue.Number, "resolution")));
        }

        [Fact]
        public void SkipsIssueThatHasInvalidResolutionLabel()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var resolutionName = "Bar";
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(resolutionName, _options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(
                error => error.Write(Messages.ReleasableIssueResolutionLabelInvalidFormat(issue.Number, resolutionName)));
        }

        [Fact]
        public void SkipsIssueThatHasNoTypeLabel()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleasableIssueLabelMissingFormat(issue.Number, "type")));
        }

        [Fact]
        public void SkipsIssueThatHasMultipleTypeLabels()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Discussion.Name, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleasableIssueLabelDuplicateFormat(issue.Number, "type")));
        }

        [Fact]
        public void SkipsIssueThatHasInvalidTypeLabel()
        {
            // Arrange
            var stubConsole = new StubConsole();

            var typeName = "Bar";
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(typeName, _options),
                });

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleasableIssueTypeLabelInvalidFormat(issue.Number, typeName)));
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenMisconfiguredIssueSkipped()
        {
            // Arrange
            var issue = GitHubStubCreator.CreateIssue("Foo");
            var sut = CreateSystemUnderTest();

            // Act
            var exception = Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(Messages.ReleasableIssuesSkippedFormat(1), exception.Message);
        }

        [Fact]
        public void ThrowsKeyNotFoundExceptionWhenLabelPrefixNotConfigured()
        {
            // Arrange
            var labelPrefix = "bar";
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateLabel(labelPrefix + _options.LabelPrefixSeparator + "Baz"),
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, _options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, _options),
                });

            var sut = CreateSystemUnderTest();

            // Act
            var exception = Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
            Assert.Equal(Messages.ReleasableIssueLabelPrefixInvalidFormat(labelPrefix), exception.Message);
        }

        [Fact]
        public void DoesNotThrowKeyNotFoundExceptionWhenLabelPrefixNotConfiguredAndFailDisabledInOptions()
        {
            // Arrange
            var options = new DeployOptions
            {
                FailOnUnconfiguredLabelPrefixes = false,
            };

            var labelPrefix = "bar";
            var issue = GitHubStubCreator.CreateIssue(
                "Foo",
                labels: new[]
                {
                    GitHubStubCreator.CreateLabel(labelPrefix + options.LabelPrefixSeparator + "Baz"),
                    GitHubStubCreator.CreateResolutionLabel(BuiltInIssueResolutions.ByDesignWontFix.Name, options),
                    GitHubStubCreator.CreateTypeLabel(BuiltInIssueTypes.Bug.Name, options),
                });

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var exception = Record.Exception(() => sut.ConvertAll(new[] { issue, }));

            // Assert
            Assert.Null(exception);
        }
    }
}
