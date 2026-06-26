import type { ReactNode } from 'react'

export type ChildrenType = Readonly<{ children: ReactNode }>

export type LogoBoxProps = {
  containerClassName?: string
  height?: number
}
