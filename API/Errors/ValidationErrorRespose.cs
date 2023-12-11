
namespace API.Errors;
public class ValidationErrorRespose : ApiResponse
{
    public ValidationErrorRespose() : base(400)
    {
    }
    public IEnumerable<string> Errors {get;set;}
}