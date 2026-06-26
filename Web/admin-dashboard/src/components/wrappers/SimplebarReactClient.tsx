import type { ChildrenType } from '@/types/component-props'
import SimpleBar, { type Props as SimpleBarProps } from 'simplebar-react'

type SimplebarReactClientProps = SimpleBarProps & ChildrenType

const SimplebarReactClient = ({ children, ...options }: SimplebarReactClientProps) => {
  return <SimpleBar {...options}>{children}</SimpleBar>
}

export default SimplebarReactClient
