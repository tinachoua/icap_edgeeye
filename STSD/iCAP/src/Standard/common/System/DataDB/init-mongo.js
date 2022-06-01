db.createUser(
    {
        user: "icap-admin",
        pwd: "icap-admin-pwd",
        roles: [
            {
                role: "readWrite",
                db: "iCAP"
            }
        ]
    }
);