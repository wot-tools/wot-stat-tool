using System;
using System.Globalization;
using System.Linq;
using LibGit2Sharp;
using System.Text.RegularExpressions;

namespace NGitVersion.Model
{
    public class Model
    {
        private readonly IRepository mRepository;

        private readonly Lazy<string> mMajor;
        private readonly Lazy<string> mMinor;
        private readonly Lazy<string> mBuild;
        private readonly Lazy<string> mRevision;
        private readonly Lazy<string> mShortHash;
        private readonly Lazy<string> mBranch;
        private readonly Lazy<string> mHasLocalChange;
        private readonly string mBuildConfig;

        private readonly string mDescription;

        public Model(IRepository repository)
        {
            mRepository = repository;
            
            mShortHash = new Lazy<string>(() => mRepository.Commits.First().Sha.Substring(0, 7));
            mBranch = new Lazy<string>(() => mRepository.Head.CanonicalName);
            mHasLocalChange = new Lazy<string>(() => mRepository.RetrieveStatus().IsDirty.ToString(CultureInfo.InvariantCulture));
#if DEBUG
            mBuildConfig = "DEBUG";
#else
            mBuildConfig = "RELEASE";
#endif

            var commit = mRepository.Commits.First();

            string hash = mRepository.Commits.First().Sha.Substring(0, 7);

            string tag = String.Empty;
            try {
                tag = repository.Describe(commit,
                    new DescribeOptions { Strategy = DescribeStrategy.Tags });
            } catch(LibGit2SharpException) {
                // use default gitDescription value
            }

            string dirty = mRepository.RetrieveStatus().IsDirty ? "-dirty" : String.Empty;

            Match descMatch = Regex.Match(tag, @"(\d+)\.(\d+)\.(\d+)");
            if(descMatch.Success)
            {
                mMajor = new Lazy<string>(() => descMatch.Groups[1].Value.ToString(CultureInfo.InvariantCulture));
                mMinor = new Lazy<string>(() => descMatch.Groups[2].Value.ToString(CultureInfo.InvariantCulture));
                mBuild = new Lazy<string>(() => descMatch.Groups[3].Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                mMajor = new Lazy<string>(() => "0".ToString(CultureInfo.InvariantCulture));
                mMinor = new Lazy<string>(() => "0".ToString(CultureInfo.InvariantCulture));
                mBuild = new Lazy<string>(() => "0".ToString(CultureInfo.InvariantCulture));
            }
            mRevision = new Lazy<string>(() => mRepository.Commits.Count().ToString(CultureInfo.InvariantCulture));
            mDescription = $"{mMajor.Value}.{mMinor.Value}.{mBuild.Value}.{mRevision.Value}{dirty}";
        }

        public string Description    { get { return mDescription; } }
        public string Company        => "wot-tools";
        public string Product        => "WoTStatTool";
        public string Copyright      => String.Empty;
        public string Trademark      => String.Empty;
        public string Culture        => String.Empty;

        public string Major          { get { return mMajor.Value; } }
        public string Minor          { get { return mMinor.Value; } }
        public string Build          { get { return mBuild.Value; } }
        public string Revision       { get { return mRevision.Value; } }

        public string ShortHash      { get { return mShortHash.Value; } }
        public string Branch         { get { return mBranch.Value; } }
        public string HasLocalChange { get { return mHasLocalChange.Value; } }
        public string BuildConfig    { get { return mBuildConfig; } }
    }
}
