export interface ResetPassword{
    loginId: string,
    passwordHint: number,
    passwordHintAnswer: string,
    newPassword: string,
    confirmNewPassword: string
}