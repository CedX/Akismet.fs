namespace Belin.Akismet

/// Specifies the result of a comment check.
type CheckResult =
  | Ham
  | Spam
  | PervasiveSpam
