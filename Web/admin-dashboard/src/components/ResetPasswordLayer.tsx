import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { Card, CardBody, Col, Container, Row } from 'react-bootstrap'
import IconifyIcon from '@/components/wrappers/IconifyIcon'
import LogoBox from '@/components/LogoBox'
import { resetPassword } from '../api/auth'

const ResetPasswordLayer = () => {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const email = searchParams.get('email') ?? ''
  const token = searchParams.get('token') ?? ''

  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const resetMutation = useMutation({
    mutationFn: () => resetPassword(email, token, newPassword),
    onSuccess: () => {
      toast.success('Password reset successfully. Please sign in.')
      navigate('/sign-in')
    },
    onError: () => {
      toast.error('Reset failed. The link may be invalid or expired.')
    },
  })

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (newPassword !== confirmPassword) {
      toast.error('Passwords do not match.')
      return
    }
    resetMutation.mutate()
  }

  const linkIsValid = Boolean(email && token)

  return (
    <div className="authentication-bg">
      <div className="account-pages pt-sm-5 pb-sm-5 py-3">
        <Container>
          <Row className="justify-content-center">
            <Col xl={5}>
              <Card className="auth-card">
                <CardBody className="px-3 py-5">
                  <LogoBox containerClassName="mx-auto mb-4 text-center auth-logo" height={52} />
                  <h2 className="fw-bold text-center fs-18">Reset Password</h2>
                  <p className="text-muted text-center mt-1 mb-4">
                    {linkIsValid
                      ? `Choose a new password for ${email}.`
                      : 'This reset link is missing required information. Please request a new one.'}
                  </p>
                  <div className="px-4">
                    {linkIsValid && (
                      <form onSubmit={handleSubmit}>
                        <div className="mb-3">
                          <label className="form-label">New Password</label>
                          <div className="input-group">
                            <span className="input-group-text">
                              <IconifyIcon icon="solar:lock-password-outline" className="fs-18" />
                            </span>
                            <input
                              type="password"
                              className="form-control"
                              placeholder="New password"
                              autoComplete="new-password"
                              value={newPassword}
                              onChange={(e) => setNewPassword(e.target.value)}
                              required
                              minLength={8}
                            />
                          </div>
                        </div>
                        <div className="mb-3">
                          <label className="form-label">Confirm Password</label>
                          <div className="input-group">
                            <span className="input-group-text">
                              <IconifyIcon icon="solar:lock-password-outline" className="fs-18" />
                            </span>
                            <input
                              type="password"
                              className="form-control"
                              placeholder="Confirm new password"
                              autoComplete="new-password"
                              value={confirmPassword}
                              onChange={(e) => setConfirmPassword(e.target.value)}
                              required
                              minLength={8}
                            />
                          </div>
                        </div>
                        <button
                          type="submit"
                          className="btn btn-primary w-100"
                          disabled={resetMutation.isPending}
                        >
                          {resetMutation.isPending ? 'Resetting...' : 'Reset Password'}
                        </button>
                      </form>
                    )}
                    <div className="text-center mt-3">
                      <Link to="/forgot-password" className="text-primary fw-semibold small">← Request a new link</Link>
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

export default ResetPasswordLayer
