This is a customized derivative of MediatR, as seen here: https://github.com/jbogard/MediatR

The primary difference is the removal of IRequest/INotification interfaces. This allows uncontrolled types or unmanaged types to be used as the request parameter. Additionally, it removes the need for request parameter types to reference the request's response type.
