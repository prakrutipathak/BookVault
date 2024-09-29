import { ReportUser } from "./reportuser.model";

export interface AdminBookReport {
    title:string,
    author:string,
    issueDate: string,
    userId: number,
    user: ReportUser
}