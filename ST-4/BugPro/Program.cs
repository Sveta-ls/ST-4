using Stateless;

namespace BugPro;

public enum BugState
{
    Open,
    Assigned,
    InProgress,
    Resolved,
    Verified,
    Closed,
    Reopened,
    Deferred,
    Duplicate,
    WonTFix,
    Invalid
}

public enum BugTrigger
{
    Assign,
    StartProgress,
    Resolve,
    Verify,
    Close,
    Reopen,
    Defer,
    MarkDuplicate,
    MarkWonTFix,
    MarkInvalid,
    Reassign,
    FailVerification
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _machine;
    private string _currentAssignee = string.Empty;

    public BugState CurrentState => _machine.State;
    public string CurrentAssignee => _currentAssignee;

    public Bug(string description)
    {
        _machine = new StateMachine<BugState, BugTrigger>(BugState.Open);
        ConfigureTransitions();
    }

    private void ConfigureTransitions()
    {
        _machine.Configure(BugState.Open)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Defer, BugState.Deferred)
            .Permit(BugTrigger.MarkDuplicate, BugState.Duplicate)
            .Permit(BugTrigger.MarkWonTFix, BugState.WonTFix)
            .Permit(BugTrigger.MarkInvalid, BugState.Invalid);

        _machine.Configure(BugState.Assigned)
            .Permit(BugTrigger.StartProgress, BugState.InProgress)
            .Permit(BugTrigger.Defer, BugState.Deferred)
            .PermitReentry(BugTrigger.Reassign);

        _machine.Configure(BugState.InProgress)
            .Permit(BugTrigger.Resolve, BugState.Resolved)
            .Permit(BugTrigger.Reassign, BugState.Assigned);

        _machine.Configure(BugState.Resolved)
            .Permit(BugTrigger.Verify, BugState.Verified)
            .Permit(BugTrigger.FailVerification, BugState.Reopened)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Verified)
            .Permit(BugTrigger.Close, BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Closed)
            .Ignore(BugTrigger.Reopen);

        _machine.Configure(BugState.Reopened)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.StartProgress, BugState.InProgress);

        _machine.Configure(BugState.Deferred)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reopen, BugState.Reopened);

        _machine.Configure(BugState.Duplicate)
            .Ignore(BugTrigger.Reopen);

        _machine.Configure(BugState.WonTFix)
            .Ignore(BugTrigger.Reopen);

        _machine.Configure(BugState.Invalid)
            .Ignore(BugTrigger.Reopen);
    }

    public void Assign(string assignee)
    {
        _currentAssignee = assignee;
        _machine.Fire(BugTrigger.Assign);
    }

    public void StartProgress() => _machine.Fire(BugTrigger.StartProgress);
    public void Resolve() => _machine.Fire(BugTrigger.Resolve);
    public void Verify() => _machine.Fire(BugTrigger.Verify);
    public void Close() => _machine.Fire(BugTrigger.Close);
    public void Reopen() => _machine.Fire(BugTrigger.Reopen);
    public void Defer() => _machine.Fire(BugTrigger.Defer);
    public void MarkDuplicate() => _machine.Fire(BugTrigger.MarkDuplicate);
    public void MarkWonTFix() => _machine.Fire(BugTrigger.MarkWonTFix);
    public void MarkInvalid() => _machine.Fire(BugTrigger.MarkInvalid);
    
    public void Reassign(string newAssignee)
    {
        _currentAssignee = newAssignee;
        _machine.Fire(BugTrigger.Reassign);
    }
    
    public void FailVerification() => _machine.Fire(BugTrigger.FailVerification);
}

class Program
{
    static void Main()
    {
        var bug = new Bug("1 бага");
       
        bug.Assign("Lady Gaga");
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        Console.WriteLine($"состояние: {bug.CurrentState}");

        bug.Close();
        var bug2 = new Bug("2 баг");
        Console.WriteLine($"новый баг: {bug2.CurrentState}");
        
        bug2.MarkDuplicate();
        Console.WriteLine($"баг отмечен как дубликат: {bug2.CurrentState}");
        
        var bug3 = new Bug("3 баг");
        bug3.MarkInvalid();
        Console.WriteLine($"баг отмечен как невалидный: {bug3.CurrentState}");
        
        var bug4 = new Bug("4 баг");
        bug4.Defer();
        bug4.Assign("Sveta Zyazeva");
        bug4.StartProgress();
        bug4.Resolve();
        bug4.Verify();
        bug4.Close();
        Console.WriteLine($"баг закрыт: {bug4.CurrentState}");
    }
}
