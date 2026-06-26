import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { Card, CardBody, Col, Container, Row } from 'react-bootstrap'
import IconifyIcon from '@/components/wrappers/IconifyIcon'
import LogoBox from '@/components/LogoBox'
import { login } from '../api/auth'
import { authStorage } from '../api/authStorage'
import DevCredentialsCheatsheet from './DevCredentialsCheatsheet'

const SignInLayer = () => {
  const navigate = useNavigate()
  const [userNameOrEmail, setUserNameOrEmail] = useState('')
  const [password, setPassword] = useState('')
  const [rememberMe, setRememberMe] = useState(true)
  const [showPassword, setShowPassword] = useState(false)

  const loginMutation = useMutation({
    mutationFn: () => login(userNameOrEmail, password),
    onSuccess: (result) => {
      authStorage.setTokens(result.accessToken, result.refreshToken, rememberMe)
      navigate('/')
    },
    onError: () => {
      toast.error('Invalid username/email or password.')
    },
  })

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    loginMutation.mutate()
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
                  <h2 className="fw-bold text-center fs-18">Sign In</h2>
                  <p className="text-muted text-center mt-1 mb-4">Enter your credentials to access LitXusCount.</p>
                  <div className="px-4">
                    <form onSubmit={handleSubmit}>
                      <div className="mb-3">
                        <label className="form-label">Username or Email</label>
                        <div className="input-group">
                          <span className="input-group-text">
                            <IconifyIcon icon="solar:user-outline" className="fs-18" />
                          </span>
                          <input
                            type="text"
                            className="form-control"
                            placeholder="Username or Email"
                            autoComplete="username"
                            value={userNameOrEmail}
                            onChange={(e) => setUserNameOrEmail(e.target.value)}
                            required
                          />
                        </div>
                      </div>
                      <div className="mb-3">
                        <label className="form-label">Password</label>
                        <div className="input-group">
                          <span className="input-group-text">
                            <IconifyIcon icon="solar:lock-password-outline" className="fs-18" />
                          </span>
                          <input
                            type={showPassword ? 'text' : 'password'}
                            className="form-control"
                            placeholder="Password"
                            autoComplete="current-password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                          />
                          <button type="button" className="input-group-text" onClick={() => setShowPassword(!showPassword)}>
                            <IconifyIcon icon={showPassword ? 'solar:eye-closed-outline' : 'solar:eye-outline'} className="fs-18" />
                          </button>
                        </div>
                      </div>
                      <div className="d-flex justify-content-between align-items-center mb-3">
                        <div className="form-check">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            id="remember-me"
                            checked={rememberMe}
                            onChange={(e) => setRememberMe(e.target.checked)}
                          />
                          <label className="form-check-label" htmlFor="remember-me">Remember me</label>
                        </div>
                        <Link to="/forgot-password" className="text-primary fw-medium small">Forgot Password?</Link>
                      </div>
                      <button
                        type="submit"
                        className="btn btn-primary w-100"
                        disabled={loginMutation.isPending}
                      >
                        {loginMutation.isPending ? 'Signing in...' : 'Sign In'}
                      </button>
                    </form>
                    <DevCredentialsCheatsheet
                      activeEmail={userNameOrEmail}
                      onSelect={(credential) => {
                        setUserNameOrEmail(credential.email)
                        setPassword(credential.password)
                      }}
                    />
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

export default SignInLayer
