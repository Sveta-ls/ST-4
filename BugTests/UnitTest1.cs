using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests;

[TestClass]
public class BugWorkflowTests
{
    [TestMethod]
    public void Test_NewBug_ShouldBeInOpenState()
    {
        var bug = new Bug("Test bug");
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Test_AssignBug_ShouldTransitionToAssigned()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
        Assert.AreEqual("Developer1", bug.CurrentAssignee);
    }

    [TestMethod]
    public void Test_StartProgress_ShouldTransitionToInProgress()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        Assert.AreEqual(BugState.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Test_ResolveBug_ShouldTransitionToResolved()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        Assert.AreEqual(BugState.Resolved, bug.CurrentState);
    }

    [TestMethod]
    public void Test_VerifyBug_ShouldTransitionToVerified()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        Assert.AreEqual(BugState.Verified, bug.CurrentState);
    }

    [TestMethod]
    public void Test_CloseBug_ShouldTransitionToClosed()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_ReopenFromResolved_ShouldTransitionToReopened()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Reopen();
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Test_DeferBug_ShouldTransitionToDeferred()
    {
        var bug = new Bug("Test bug");
        bug.Defer();
        Assert.AreEqual(BugState.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void Test_MarkDuplicate_ShouldTransitionToDuplicate()
    {
        var bug = new Bug("Test bug");
        bug.MarkDuplicate();
        Assert.AreEqual(BugState.Duplicate, bug.CurrentState);
    }

    [TestMethod]
    public void Test_MarkInvalid_ShouldTransitionToInvalid()
    {
        var bug = new Bug("Test bug");
        bug.MarkInvalid();
        Assert.AreEqual(BugState.Invalid, bug.CurrentState);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_InvalidTransition_StartProgressWithoutAssign_ShouldThrowException()
    {
        var bug = new Bug("Test bug");
        bug.StartProgress(); //1
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_InvalidTransition_ResolveWithoutStartingProgress_ShouldThrowException()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.Resolve();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_InvalidTransition_VerifyWithoutResolving_ShouldThrowException()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Verify();
    }

    [TestMethod]
    public void Test_FailVerification_ShouldTransitionToReopened()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.FailVerification();
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Test_ReassignBug_ShouldStayInAssignedState()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.Reassign("Developer2");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
        Assert.AreEqual("Developer2", bug.CurrentAssignee);
    }

    [TestMethod]
    public void Test_ClosedBug_ShouldIgnoreReopen()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.Reopen();
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_MarkWonTFix_ShouldTransitionToWonTFix()
    {
        var bug = new Bug("Test bug");
        bug.MarkWonTFix();
        Assert.AreEqual(BugState.WonTFix, bug.CurrentState);
    }

    [TestMethod]
    public void Test_DeferredBug_CanBeAssigned()
    {
        var bug = new Bug("Test bug");
        bug.Defer();
        bug.Assign("Developer1");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_CompleteWorkflow_HappyPath()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_ReopenedBug_CanBeReassigned()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Resolve();
        bug.Reopen();
        bug.Assign("Developer2");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_ReassignFromInProgress_ShouldTransitionToAssigned()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.StartProgress();
        bug.Reassign("Developer2");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Test_DoubleAssignment_ShouldStayInAssignedState()
    {
        var bug = new Bug("Test bug");
        bug.Assign("Developer1");
        bug.Reassign("Developer2");
        bug.Reassign("Developer3");
        Assert.AreEqual(BugState.Assigned, bug.CurrentState);
        Assert.AreEqual("Developer3", bug.CurrentAssignee);
    }
}
