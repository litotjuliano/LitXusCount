import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Link } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { toast } from "react-toastify";
import { forgotPassword } from "../api/auth";

const ForgotPasswordLayer = () => {
  const [email, setEmail] = useState("");
  const [devResetLink, setDevResetLink] = useState<string | null>(null);

  const forgotPasswordMutation = useMutation({
    mutationFn: () => forgotPassword(email),
    onSuccess: (result) => {
      toast.success(result.message);
      setDevResetLink(result.devResetLink);
    },
    onError: () => {
      toast.error("Something went wrong. Please try again.");
    },
  });

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    forgotPasswordMutation.mutate();
  };

  return (
    <section className='auth bg-base d-flex flex-wrap'>
      <div className='auth-left d-lg-block d-none'>
        <div className='d-flex align-items-center flex-column h-100 justify-content-center'>
          <img src='assets/images/auth/forgot-pass-img.png' alt='' />
        </div>
      </div>
      <div className='auth-right py-32 px-24 d-flex flex-column justify-content-center'>
        <div className='max-w-464-px mx-auto w-100'>
          <div>
            <Link to='/' className='mb-40 max-w-290-px'>
              <img src='assets/images/logo.png' alt='' />
            </Link>
            <h4 className='mb-12'>Forgot Password</h4>
            <p className='mb-32 text-secondary-light text-lg'>
              Enter the email address linked to your account and we'll send you a link to reset
              your password.
            </p>
          </div>
          <form onSubmit={handleSubmit}>
            <div className='icon-field mb-16'>
              <span className='icon top-50 translate-middle-y'>
                <Icon icon='mage:email' />
              </span>
              <input
                type='email'
                className='form-control h-56-px bg-neutral-50 radius-12'
                placeholder='lito@juliano.com'
                autoComplete='email'
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <button
              type='submit'
              className='btn btn-primary text-sm btn-sm px-12 py-16 w-100 radius-12 mt-12'
              disabled={forgotPasswordMutation.isPending}
            >
              {forgotPasswordMutation.isPending ? "Sending..." : "Send Reset Link"}
            </button>
            {devResetLink && (
              <div className='mt-16 p-12 radius-8 bg-warning-focus border border-warning-main text-sm'>
                <strong>Dev only</strong> — no email sender is wired up yet, so here's the link
                directly:{" "}
                <Link to={devResetLink.replace(window.location.origin, "")} className='fw-semibold'>
                  Open reset link
                </Link>
              </div>
            )}
            <div className='mt-32 text-center text-sm'>
              <Link to='/sign-in' className='text-primary-600 fw-semibold'>
                Back to Sign In
              </Link>
            </div>
          </form>
        </div>
      </div>
    </section>
  );
};

export default ForgotPasswordLayer;
