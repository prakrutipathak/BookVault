import { ReportUser } from "./reportuser.model";

export interface AdminReportUser {

    userId: number,
    issueDate: string,
    user: ReportUser
}