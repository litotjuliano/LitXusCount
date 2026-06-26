import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { Card, CardBody, Col, Container, Row } from 'react-bootstrap'
import IconifyIcon from '@/components/wrappers/IconifyIcon'
import LogoBox from '@/components/LogoBox'
import { forgotPassword } from '../api/auth'

const ForgotPasswordLayer = () => {
  const [email, setEmail] = useState('')
  const [devResetLink, setDevResetLink] = useState<string | null>(null)

  const forgotPasswordMutation = useMutation({
    mutationFn: () => forgotPassword(email),
    onSuccess: (result) => {
      toast.success(result.message)
      setDevResetLink(result.devResetLink)
    },
    onError: () => {
      toast.error('Something went wrong. Please try again.')
    },
  })

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    forgotPasswordMutation.mutate()
  }

  return (
    <div className="authentication-bg">
      <div className="account-pages pt-sm-5 pb-sm-5 py-3">
        <Container>
          <Row className="justify-content-center">
            <Col xl={5}>
              <Card className="auth-card">
                <CardBody className="px-3 py-5">
                  <LogoBox containerClassName="mx-auto mb-4 text-center auth-logo" height={52} />
                  <h2 className="fw-bold text-center fs-18">Forgot Password</h2>
                  <p className="text-muted text-center mt-1 mb-4">
                    Enter the email address linked to your account and we'll send a reset link.
                  </p>
                  <div className="px-4">
                    <form onSubmit={handleSubmit}>
                      <div className="mb-3">
                        <label className="form-label">Email Address</label>
                        <div className="input-group">
                          <span className="input-group-text">
                            <IconifyIcon icon="solar:letter-outline" className="fs-18" />
                          </span>
                          <input
                            type="email"
                            className="form-control"
                            placeholder="your@email.com"
                            autoComplete="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                          />
                        </div>
                      </div>
                      <button
                        type="submit"
                        className="btn btn-primary w-100 mb-3"
                        disabled={forgotPasswordMutation.isPending}
                      >
                        {forgotPasswordMutation.isPending ? 'Sending...' : 'Send Reset Link'}
                      </button>
                      {devResetLink && (
                        <div className="alert alert-warning small">
                          <strong>Dev only</strong> — reset link:{' '}
                          <Link to={devResetLink.replace(window.location.origin, '')} className="fw-semibold">
                            Open reset link
                          </Link>
                        </div>
                      )}
                    </form>
                    <div className="text-center mt-3">
                      <Link to="/sign-in" className="text-primary fw-semibold small">← Back to Sign In</Link>
                    </div>
                  </div>
                </CardBody>
              </Card>
            </Col>
          </Row>
        </Container>
      </div>
    </div>
  )
}

export default ForgotPasswordLayer
