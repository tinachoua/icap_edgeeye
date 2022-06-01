import Joi from 'joi';

const serialNumbersSchema = Joi.object({
    sn: Joi.array().items(Joi.string().required()).required(),
});

const singleSNSchema = Joi.object({
    sn: Joi.string().required(),
});

const InnoAGESchemas = {
    '/InnoAGE/unsubscribe': serialNumbersSchema,
    '/InnoAGE/recovery': singleSNSchema,
    '/InnoAGE/reboot': singleSNSchema,
    '/InnoAGE/power-switch': singleSNSchema,
}

// export the schemas
export default InnoAGESchemas;